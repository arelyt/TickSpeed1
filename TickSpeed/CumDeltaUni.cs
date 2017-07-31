using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты по честному ресемплингу.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumDeltaUni")]
#pragma warning restore 612
    public class CumDeltaUni : IBar2DoubleHandler
    {
        public static IList<double> Cacheflow { get; set; }

        public interface ICumDeltaUni
        {
            // ReSharper disable once InconsistentNaming
            double[] CumDeltaUni(double[] in1, int in2, int in3);
        }

        [HandlerParameter(Name = "CumDeltaUni", Default = "Volume", NotOptimized = true)]
        public V2.CumDeltaType Type { get; set; }
        public IList<double> Execute(ISecurity security)
        {
            bool type;
            switch (Type)
            {
                case V2.CumDeltaType.Volume:
                    type = true;
                    break;
                case V2.CumDeltaType.Tick:
                    type = false;
                    break;
                default:
                    type = true;
               break;
            }
            var count = security.Bars.Count;
            if (count < 2)
                return null;

            var values = new double[count];
            var time = new double[count];
            var temp = new double[count];
            values[0] = 0;
            if (type)
            {
                for (var i = 1; i < count; i++)
                {
                    var trades = security.GetTrades(i);
                    var buyVolume = trades.Where(trd => trd.Direction == TradeDirection.Buy).Sum(trd => trd.Quantity);
                    var sellVolume = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                    var cumvolume = buyVolume - sellVolume;
                    values[i] = values[i - 1] + cumvolume;
                    time[i] =security.Bars[i].Date.TimeOfDay.TotalSeconds;

                }
            }
            else
            {
                for (var i = 1; i < count; i++)
                {
                    var trades = security.GetTrades(i);
                    var valueTickBuy = trades.Count(trd => trd.Direction == TradeDirection.Buy);
                    var valueTickSell = trades.Count(trd => trd.Direction == TradeDirection.Sell);
                    var cumtick = valueTickBuy - valueTickSell;
                    values[i] = values[i - 1] + cumtick;
                    time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds;

                }
            }
            // Искусственное добавление микросекунд для одинаковых тиков
            Array.Copy(time, temp, count);
            var vector = new List<int>();
            //var delta = 1e-4;
            for (int i = 1; i < count-1; i++)
            {
                if (time[i] > time[i-1] && time[i] < time[i+1])
                {
                    continue;
                }
                else if  ((Equals(time[i], time[i-1]) && Equals(time[i], time[i+1]))
                            || (time[i] > time[i-1] && Equals(time[i], time[i+1])))
                    {
                        vector.Add(i);
                    }
                else if (Equals(time[i], time[i-1]) && time[i] < time[i+1])
                {
                    vector.Add(i);
                    var delta = 1e-4 / vector.Count;
                    var l = 1.0;
                    foreach (var j in vector)
                    {
                        temp[j] = temp[j] + l * delta;
                        l = l + 1;
                    }
                    vector.Clear();
                }
                
            }
            if (Equals(time[count-1], time[count-2]))
            {
                temp[count - 1] = temp[count - 1] + 1e-4;
            }
            // Теперь вызов процедуры ресемплинга и сглаживания в матлаб

            return values;
            

        }
    }
}

