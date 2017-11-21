using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
namespace TickSpeed
{
    // Инкрементальный SSA.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2_1N2")]
#pragma warning restore 612


    public class IncrementalSSA1N2 : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;

        // количество последних окон, которые перезаписываются при анализе
        const int overwrite_windows = 2;

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static alglib.ssamodel worker2;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static alglib.ssamodel analyzer2;

        // последний сглаженный результат
        private static double[] last_result2;

        // количество данных в моделях
        private static int data_inside2;

        // инициализация моделей
        static IncrementalSSA1N2()
        {
            double[,] dummy_basis = new double[,] { { 1 } };
            data_inside2 = 0;
            last_result2 = new double[0];
            alglib.ssacreate(out worker2);
            alglib.ssacreate(out analyzer2);
            int current_window = 1;
            int current_k = 1;
            alglib.ssasetwindow(worker2, current_window);
            alglib.ssasetwindow(analyzer2, current_window);
            alglib.ssasetalgotopkrealtime(worker2, current_k);
            alglib.ssasetpoweruplength(worker2, 5);
            //alglib.ssasetalgotopkdirect(worker, current_k);
            alglib.ssasetalgoprecomputed(analyzer2, dummy_basis, current_window, current_k);
        }

        [HandlerParameter(true, "100", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
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
            alglib.ssasetwindow(worker2, window_size);
            alglib.ssasetalgotopkrealtime(worker2, k);
            if (data_inside2 > 0)
            {
                // режим обновления
                for (int i = data_inside2; i < count; i++)
                {
                    alglib.ssaappendpointandupdate(worker2, myDoubles[i], i == count - 1 ? update_freq : 0.0);
                    alglib.ssaappendpointandupdate(analyzer2, myDoubles[i], 0.0);
                }
            }
            else
            {
                // режим изначального создания
                double[] vals = new double[count];
                for (int i = 0; i < count; i++)
                    vals[i] = myDoubles[i];
                alglib.ssaaddsequence(worker2, vals, count);
                alglib.ssaaddsequence(analyzer2, vals, count);
            }
            data_inside2 = count;
            alglib.ssagetbasis(worker2, out new_basis, out sv, out dummy0, out dummy1);
            alglib.ssasetalgoprecomputed(analyzer2, new_basis, window_size, k);

            // анализ
            int alen = (overwrite_windows + 1) * window_size; // +1 позволяет сгладить шум в начале окна
            double[] last_trend, last_noise;
            alglib.ssaanalyzelast(analyzer2, alen, out last_trend, out last_noise);

            // результат
            int olen = overwrite_windows * window_size;
            double[] result = new double[count + Numfor];
            for (int i = 0; i < last_result2.Length; i++)
                result[i] = last_result2[i];
            for (int i = last_result2.Length; i < count; i++)
                result[i] = myDoubles[i];
            for (int i = count - Math.Min(olen, count); i < count; i++)
                result[i] = last_trend[alen + (i - count)];
            if (Numfor > 0)
            {
                double[] fc;
                alglib.ssaforecastlast(analyzer2, Numfor, out fc);
                for (int i = 0; i < Numfor; i++)
                    result[count + i] = fc[i];
            }

            // кэшировать сглаженный тренд, предсказание не кешируем
            last_result2 = new double[count];
            for (int i = 0; i < count; i++)
                last_result2[i] = result[i];
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Context.Log("ssaV2_1 exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}