using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTOSHIB")]
    public class VolTickOscShib : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "kVOL", Default = "0.4", Min = "0.1", Max = "1.0", Step = "0.1")]
        public double KVol { get; set; }

        [HandlerParameter(Name = "kTC", Default = "0.6", Min = "0.1", Max = "1.0", Step = "0.1")]
        public double KTc { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy  = 0.0;
                var valueTickSell = 0.0;
                var valueVolBuy   = 0.0;
                var valueVolSell  = 0.0;

                foreach (var t in trades)
                {
                    var trd = t;
                    valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                    valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                }
                // Считаем осциллятор

                values[i] = KTc * ((valueTickBuy  - valueTickSell) / (valueTickBuy + valueTickSell)) +
                            KVol * ((valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell));
            }
            return values;
        }
    }
}