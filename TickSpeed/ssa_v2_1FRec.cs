using Altaxo.Calc;
using System;
using System.Collections.Generic;
using System.Globalization;
using Altaxo.Collections;
using TSLab.Script.Handlers;
namespace TickSpeed
{
    // Инкрементальный SSA на фоне реала через квант времени.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2_1FRec")]
#pragma warning restore 612


    public class IncrementalSSA1Frec : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static readonly alglib.ssamodel worker3;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static readonly alglib.ssamodel analyzer3;

        // последний сглаженный результат
        private static double[] last_result3;

        // последний сглаженный результат
        private static double[] fc_last3;

        // Последнее количество отсчетов при пересчете прогноза
        private static int count_last;

        // Время конструктора класса
        private static DateTime _timestart = DateTime.Now;
        // количество данных в моделях
        private static int data_inside3;

        // инициализация моделей
        static IncrementalSSA1Frec()
        {
            double[,] dummy_basis = new double[,] { { 1 } };
            data_inside3 = 0;
            last_result3 = new double[0];
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

        [HandlerParameter(true, "100", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        [HandlerParameter(true, "10", Name = "Counter", Max = "100", Min = "1", Step = "1", NotOptimized = false)]
        public int Counter { get; set; }

        [HandlerParameter(true, "2", Name = "QWin", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        // количество последних окон, которые перезаписываются при анализе
        public int overwrite_windows3 { get; set; }
    

        public IList<double> Execute(IList<double> myDoubles)
        {
            // Проверка на то, что конструктор класса и индикатор отработали хотя бы  10 сек
            var t = DateTime.Now;
            var ctx = Context;
            if ((t - _timestart).Seconds < 10 )
            {
                return myDoubles;
            }
            //Cf = myDoubles.Count - (myDoubles.Count % Counter);
            for (int i = 0; i < myDoubles.Count; i++)
            {
                if (myDoubles[i].IsNaN())
                {
                    myDoubles[i] = 0;
                }
            }
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
            alglib.ssasetwindow(worker3, window_size);
            alglib.ssasetalgotopkrealtime(worker3, k);
            if (data_inside3 > 0)
            {
                // режим обновления
                for (int i = data_inside3; i < count; i++)
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
                alglib.ssaaddsequence(worker3, vals, count);
                alglib.ssaaddsequence(analyzer3, vals, count);
            }
            data_inside3 = count;
            alglib.ssagetbasis(worker3, out new_basis, out sv, out dummy0, out dummy1);
            alglib.ssasetalgoprecomputed(analyzer3, new_basis, window_size, k);

            // анализ
            int alen = (overwrite_windows3 + 1) * window_size; // +1 позволяет сгладить шум в начале окна
            double[] last_trend, last_noise;
            alglib.ssaanalyzelast(analyzer3, alen, out last_trend, out last_noise);

            // результат
            int olen = overwrite_windows3 * window_size;
            double[] result = new double[count + Numfor];
            for (int i = 0; i < last_result3.Length; i++)
                result[i] = last_result3[i];
            for (int i = last_result3.Length; i < count; i++)
                result[i] = myDoubles[i];
            for (int i = count - Math.Min(olen, count); i < count; i++)
                result[i] = last_trend[alen + (i - count)];

            // Прогнозируем только каждые Counter пересчетов и если Numfor не ноль
            if (Numfor > 0)
            {
                
                if (count % Counter ==0)
                {
                    double[] fc;
                    alglib.ssaforecastlast(analyzer3, Numfor, out fc);
                    count_last = count;
                    fc_last3 = fc;
                    ctx.StoreObject("forecast", fc_last3);
                }
               
                var vv = (IList<double>)ctx.LoadObject("forecast");
                if (count - count_last != 0)
                {
                    Array.Resize(ref result, count + Numfor - count_last);
                }
                // Наползающий на остаток прогноза реал
                for (int i = 0; i < Numfor - (count - count_last); i++)
                    result[count + i] = vv[count - count_last + i];
                
            }

            // кэшировать сглаженный тренд, предсказание не кешируем
            last_result3 = new double[count];
            for (int i = 0; i < count; i++)
                last_result3[i] = result[i];
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Context.Log("ssaV2_1 exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}