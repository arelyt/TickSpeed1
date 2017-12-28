using Altaxo.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using MoreLinq;
using RusAlgo.Helper;
using TSLab.Script.Handlers;
namespace TickSpeed
{
    // Инкрементальный SSA.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2_1_2")]
#pragma warning restore 612


    public class IncrementalSSA1_2 : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;

        

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static readonly alglib.ssamodel worker;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static readonly alglib.ssamodel analyzer;

        // последний сглаженный результат
        private static double[] last_result;

        // количество данных в моделях
        private static int data_inside;

        // инициализация моделей
        static IncrementalSSA1_2()
        {
            alglib.alloc_counter_activate();
            alglib.set_dbg_flag(1, 1);

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
            ////////////////////////////////alglib.ssasetalgotopkdirect(worker, current_k);
            alglib.ssasetalgoprecomputed(analyzer, dummy_basis, current_window, current_k);
        }

        [HandlerParameter(true, "100", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        [HandlerParameter(true, "2", Name = "QWin", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        // количество последних окон, которые перезаписываются при анализе
        public int overwrite_windows { get; set; }

        [HandlerParameter(true, "fore", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }

        //[HandlerParameter(Name = "Reset", Default = "true", NotOptimized = false)]
        //public bool Reset { get; set; }


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
            alglib.ssasetwindow(worker, window_size);
            alglib.ssasetalgotopkrealtime(worker, k);
            if (data_inside > 0)
            {
                // режим обновления
                for (int i = data_inside; i < count; i++)
                {
                    var svd = DateTime.Now;
                    alglib.ssaappendpointandupdate(worker, myDoubles[i], i == count - 1 ? update_freq : 0.0);
                    var gvd = (DateTime.Now - svd).TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    Context.Log("svd exec for " + gvd + " msec", MessageType.Info, toMessageWindow: true);
                    alglib.ssaappendpointandupdate(analyzer, myDoubles[i], 0.0);
                }
            }
            else
            {
                // режим изначального создания
                double[] vals = new double[count];
                for (int i = 0; i < count; i++)
                    vals[i] = myDoubles[i];
                alglib.ssacleardata(worker);
                alglib.ssacleardata(analyzer);
                alglib.ssaaddsequence(worker, vals, count);
                alglib.ssaaddsequence(analyzer, vals, count);
                last_result = new double[0];
                need_full_analysis = true;
            }
            data_inside = count;
            alglib.ssagetbasis(worker, out new_basis, out sv, out dummy0, out dummy1);
            alglib.ssasetalgoprecomputed(analyzer, new_basis, window_size, k);

            // анализ
            int alen = need_full_analysis ? count : Math.Max(count - last_result.Length + window_size, overwrite_windows * window_size + window_size); // +window_size позволяет сгладить шум в начале окна
            double[] last_trend, last_noise;
            alglib.ssaanalyzelast(analyzer, alen, out last_trend, out last_noise);

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
                alglib.ssaforecastlast(analyzer, Numfor, out fc);
                for (int i = 0; i < Numfor; i++)
                    result[count + i] = fc[i];
            }

            // кэшировать сглаженный тренд, предсказание не кешируем
            last_result = new double[count];
            for (int i = 0; i < count; i++)
                last_result[i] = result[i];
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Context.Log("ssaV2 exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}