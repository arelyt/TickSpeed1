using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Инкрементальный SSA.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2")]
#pragma warning restore 612


    public class IncrementalSSA : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;

        // количество последних окон, которые перезаписываются при анализе
        const int overwrite_windows = 3;

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static alglib.ssamodel worker;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static alglib.ssamodel analyzer;

        // последний сглаженный результат
        private static double[] last_result;

        // количество данных в моделях
        private static int data_inside;

        // инициализация моделей
        static IncrementalSSA()
        {
            double[,] dummy_basis = new double[,] { { 1 } };
            data_inside = 0;
            last_result = new double[0];
            alglib.ssacreate(out worker);
            alglib.ssacreate(out analyzer);
            int current_window = 1;
            int current_k = 1;
            alglib.ssasetwindow(worker, current_window);
            alglib.ssasetwindow(analyzer, current_window);
            alglib.ssasetalgotopkrealtime(worker, current_k);
            alglib.ssasetpoweruplength(worker, 5);
            //alglib.ssasetalgotopkdirect(worker, current_k);
            alglib.ssasetalgoprecomputed(analyzer, dummy_basis, current_window, current_k);
        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var t = DateTime.Now;
            // вырожденные случаи
            if (myDoubles == null)
                return myDoubles;
            int count = myDoubles.Count;
            if (count < Numdec + 2)
                return myDoubles;

            // нормализация параметров
            int window_size = Math.Max((int)Math.Round(Numdec), 1);
            int k = Math.Max((int)Math.Round(Numrec), 1);

            // обновление объектов worker и analyzer, обновление датасетов
            int dummy0, dummy1;
            double[,] new_basis;
            double[] sv;
            alglib.ssasetwindow(worker, window_size);
            alglib.ssasetalgotopkrealtime(worker, k);
            if (data_inside > 0)
            {
                // режим обновления
                for (int i = data_inside; i < count; i++)
                {
                    alglib.ssaappendpointandupdate(worker, myDoubles[i], i == count - 1 ? update_freq : 0.0);
                    alglib.ssaappendpointandupdate(analyzer, myDoubles[i], 0.0);
                }
            }
            else
            {
                // режим изначального создания
                double[] vals = new double[count];
                for (int i = 0; i < count; i++)
                    vals[i] = myDoubles[i];
                Context.Log("trampampam" + " msec", MessageType.Info, toMessageWindow: true);
                alglib.ssaaddsequence(worker, vals, count);
                alglib.ssaaddsequence(analyzer, vals, count);
            }
            data_inside = count;
            alglib.ssagetbasis(worker, out new_basis, out sv, out dummy0, out dummy1);
            alglib.ssasetalgoprecomputed(analyzer, new_basis, window_size, k);

            // анализ
            int alen = (overwrite_windows + 1) * window_size; // +1 позволяет сгладить шум в начале окна
            double[] last_trend, last_noise;
            alglib.ssaanalyzelast(analyzer, alen, out last_trend, out last_noise);

            // результат
            int olen = overwrite_windows * window_size;
            double[] result = new double[count];
            for (int i = 0; i < last_result.Length; i++)
                result[i] = last_result[i];
            for (int i = last_result.Length; i < count; i++)
                result[i] = myDoubles[i];
            for (int i = count - Math.Min(olen, count); i < count; i++)
                result[i] = last_trend[alen + (i - count)];
            last_result = (double[])result.Clone();
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Context.Log("ssaV2 exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}