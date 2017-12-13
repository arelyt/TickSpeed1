using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Collections;
using MoreLinq;
using TSLab.Script.Handlers;
using RusAlgo.Helper;
namespace TickSpeed
{
    // Инкрементальный SSA.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA_V2_1N1")]
#pragma warning restore 612


    public class IncrementalSSA1N1 : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        // частота обновления при поступлении новой точки
        const double update_freq = 1.0;       

        // worker - модель, которая строит базис, возможно - в фоновом режиме
        private static alglib.ssamodel worker1;

        // analyzer - модель, которая делает анализ на основе базиса, построенного worker
        private static alglib.ssamodel analyzer1;

        // последний сглаженный результат
        private static double[] last_result1;

        // количество данных в моделях
        private static int data_inside1;

        // инициализация моделей
        static IncrementalSSA1N1()
        {
            double[,] dummy_basis = new double[,] { { 1 } };
            data_inside1 = 0;
            last_result1 = new double[0];
            alglib.ssacreate(out worker1);
            alglib.ssacreate(out analyzer1);
            int current_window = 1;
            int current_k = 1;
            alglib.ssasetwindow(worker1, current_window);
            alglib.ssasetwindow(analyzer1, current_window);
            alglib.ssasetalgotopkrealtime(worker1, current_k);
            alglib.ssasetpoweruplength(worker1, 5);
            //alglib.ssasetalgotopkdirect(worker, current_k);
            alglib.ssasetalgoprecomputed(analyzer1, dummy_basis, current_window, current_k);
        }

        [HandlerParameter(true, "100", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        [HandlerParameter(true, "2", Name = "QWin", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        // количество последних окон, которые перезаписываются при анализе
        public int overwrite_windows1 { get; set; }

        [HandlerParameter(true, "5", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Order { get; set; }

        [HandlerParameter(true, "61", Name = "winSG", Max = "120", Min = "1", Step = "1", NotOptimized = false)]
        public int WinSg { get; set; }

        [HandlerParameter(true, "0", Name = "deriv", Max = "3", Min = "0", Step = "1", NotOptimized = true)]
        public int Deriv { get; set; }

        [HandlerParameter(true, "fore", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }

        [HandlerParameter(true, "false", Name = "Reset", NotOptimized = false)]
        public bool Reset { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var t = DateTime.Now;
            for (int i = 0; i < myDoubles.Count; i++)
            {
                if (RMath.IsNaN(myDoubles[i]))
                {
                    myDoubles[i] = 0;
                }
            }
            // вырожденные случаи
            //if (myDoubles == null)
            //    return myDoubles;
            if (Reset)
            {
                double[,] dummy_basis = new double[,] {{1}};
                data_inside1 = 0;
                last_result1 = new double[0];
                alglib.ssacreate(out worker1);
                alglib.ssacreate(out analyzer1);
                int current_window = 1;
                int current_k = 1;
                alglib.ssasetwindow(worker1, current_window);
                alglib.ssasetwindow(analyzer1, current_window);
                alglib.ssasetalgotopkrealtime(worker1, current_k);
                alglib.ssasetpoweruplength(worker1, 5);
                //alglib.ssasetalgotopkdirect(worker, current_k);
                alglib.ssasetalgoprecomputed(analyzer1, dummy_basis, current_window, current_k);
                return myDoubles;
            }
            else
            {


                int count = myDoubles.Count;
                if (count < Numdec + 2)
                    return myDoubles;

                // нормализация параметров
                int window_size = Math.Max((int) Math.Round(Numdec), 1);
                int k = Math.Max((int) Math.Round(Numrec), 1);

                // обновление объектов worker и analyzer, обновление датасетов
                int dummy0, dummy1;
                double[,] new_basis;
                double[] sv;
                alglib.ssasetwindow(worker1, window_size);
                alglib.ssasetalgotopkrealtime(worker1, k);
                if (data_inside1 > 0)
                {
                    // режим обновления
                    for (int i = data_inside1; i < count; i++)
                    {
                        alglib.ssaappendpointandupdate(worker1, myDoubles[i], i == count - 1 ? update_freq : 0.0);
                        alglib.ssaappendpointandupdate(analyzer1, myDoubles[i], 0.0);
                    }
                }
                else
                {
                    // режим изначального создания
                    double[] vals = new double[count];
                    for (int i = 0; i < count; i++)
                        vals[i] = myDoubles[i];
                    alglib.ssaaddsequence(worker1, vals, count);
                    alglib.ssaaddsequence(analyzer1, vals, count);
                }
                data_inside1 = count;
                alglib.ssagetbasis(worker1, out new_basis, out sv, out dummy0, out dummy1);
                alglib.ssasetalgoprecomputed(analyzer1, new_basis, window_size, k);

                // анализ
                int alen = (overwrite_windows1 + 1) * window_size; // +1 позволяет сгладить шум в начале окна
                double[] last_trend, last_noise;
                alglib.ssaanalyzelast(analyzer1, alen, out last_trend, out last_noise);

                // результат
                int olen = overwrite_windows1 * window_size;
                double[] result = new double[count + Numfor];
                double[] values = new double[count + Numfor];
                for (int i = 0; i < last_result1.Length; i++)
                    result[i] = last_result1[i];
                for (int i = last_result1.Length; i < count; i++)
                    result[i] = myDoubles[i];
                for (int i = count - Math.Min(olen, count); i < count; i++)
                    result[i] = last_trend[alen + (i - count)];
                if (Numfor > 0)
                {
                    double[] fc;
                    //alglib.ssaforecastlast(analyzer, Numfor, out fc);
                    alglib.ssaforecastavglast(analyzer1, 5, Numfor, out fc);
                    for (int i = 0; i < Numfor; i++)
                        result[count + i] = fc[i];

                    var rt = (IList<double>) Context.LoadObject(Objname);
                    if (rt.IsNull() || rt.Count < count)
                    {
                        var tt = new double[count];
                        for (int i = 0; i < count; i++)
                        {
                            tt[i] = 0;
                        }
                        var tr = tt.ToList();
                        tr.AddRange(fc);
                        Context.StoreObject(Objname, tr);
                    }
                    else
                    {
                        var vt = (IList<double>) Context.LoadObject(Objname);
                        var ct = vt.Count;
                        var vb = fc.TakeLast(count + Numfor - ct);
                        vt.AddRange(vb);
                        //vt.TakeLast(vt.Count - 1);
                        Context.StoreObject(Objname, vt);
                    }


                }

                // кэшировать сглаженный тренд, предсказание не кешируем
                last_result1 = new double[count];
                for (int i = 0; i < count; i++)
                    last_result1[i] = result[i];
                SavitzkyGolay sg = new SavitzkyGolay(WinSg, Deriv, Order);
                sg.Apply(result, values);
                var g = (DateTime.Now - t).TotalMilliseconds.ToString(System.Globalization.CultureInfo
                    .InvariantCulture);
                Context.Log("ssaV2_1 exec for !!!" + g + " msec", MessageType.Info, toMessageWindow: true);
                return values;
            }
        }
    }
}