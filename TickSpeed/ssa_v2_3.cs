using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Инкрементальный SSA.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2_3")]
#pragma warning restore 612


    public class IncrementalSSA3 : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;

        // количество последних окон, которые перезаписываются при анализе
        const int overwrite_windows = 2;

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static alglib.ssamodel worker3;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static alglib.ssamodel analyzer3;

        // последний сглаженный результат
        private static double[] last_result;

        // количество данных в моделях
        private static int data_inside;

        // инициализация моделей
        static IncrementalSSA3()
        {
            double[,] dummy_basis = new double[,] { { 1 } };
            data_inside = 0;
            last_result = new double[0];
            alglib.ssacreate(out worker3);
            alglib.ssacreate(out analyzer3);
            int current_window = 1;
            int current_k = 1;
            alglib.ssasetwindow(worker3, current_window);
            alglib.ssasetwindow(analyzer3, current_window);
            alglib.ssasetalgotopkrealtime(worker3, current_k);
            alglib.ssasetpoweruplength(worker3, 5);
            //alglib.ssasetalgotopkdirect(worker, current_k);
            alglib.ssasetalgoprecomputed(analyzer3, dummy_basis, current_window, current_k);
        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        [HandlerParameter(Name = "Reset", Default = "true", NotOptimized = false)]
        public bool Reset { get; set; }

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
            bool need_full_analysis = false;
            alglib.ssasetwindow(worker3, window_size);
            alglib.ssasetalgotopkrealtime(worker3, k);
            if (Reset)
            {
                data_inside = 0;
            }
            if (data_inside > 0)
            {
                // режим обновления
                for (int i = data_inside; i < count; i++)
                {
                    alglib.ssaappendpointandupdate(worker3, myDoubles[i], i == count - 1 ? update_freq : 0.0);
                    alglib.ssaappendpointandupdate(analyzer3, myDoubles[i], 0.0);
                }
            }
            else
            {
                // режим изначального создания
                double[] vals = new double[count];
                for (int i = 0; i < count; i++)
                    vals[i] = myDoubles[i];
                alglib.ssacleardata(worker3);
                alglib.ssacleardata(analyzer3);
                alglib.ssaaddsequence(worker3, vals, count);
                alglib.ssaaddsequence(analyzer3, vals, count);
                last_result = new double[0];
                need_full_analysis = true;
            }
            data_inside = count;
            alglib.ssagetbasis(worker3, out new_basis, out sv, out dummy0, out dummy1);
            alglib.ssasetalgoprecomputed(analyzer3, new_basis, window_size, k);

            // анализ
            int alen = need_full_analysis ? count : Math.Max(count - last_result.Length + window_size, overwrite_windows * window_size + window_size); // +window_size позволяет сгладить шум в начале окна
            double[] last_trend, last_noise;
            alglib.ssaanalyzelast(analyzer3, alen, out last_trend, out last_noise);

            // результат
            int olen = need_full_analysis ? count : alen - window_size;
            double[] result = new double[count + Numfor];
            for (int i = 0; i < last_result.Length; i++)
                result[i] = last_result[i];
            for (int i = count - Math.Min(olen, count); i < count; i++)
                result[i] = last_trend[alen + (i - count)];
            if (Numfor > 0)
            {
                double[] fc;
                alglib.ssaforecastlast(analyzer3, Numfor, out fc);
                for (int i = 0; i < Numfor; i++)
                    result[count + i] = fc[i];
            }

            // кэшировать сглаженный тренд, предсказание не кешируем
            last_result = new double[count];
            for (int i = 0; i < count; i++)
                last_result[i] = result[i];
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssaV2_3exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }

        public IContext Context { get; set; }
    }

}