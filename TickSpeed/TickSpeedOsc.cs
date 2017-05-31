using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("TSO")]
#pragma warning restore 612
    public class TickSpeedOsc : IBar2DoubleHandler
    {
        
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            var datme = new double[count];
            if (count < 2)
                return null;

            values[0] = 1;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy  = 0.0;
                var valueTickSell = 0.0;
                datme[i] = TimeSpan.FromTicks(security.Bars[i].Date.Ticks - security.Bars[i - 1].Date.Ticks).TotalSeconds;
                foreach (var t in trades)
                {
                    valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                    valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                    
                }
                // Считаем осциллятор

                values[i] = Math.Tanh((valueTickBuy - valueTickSell) /
                                 (valueTickBuy + valueTickSell));
            }
            return values;
        }
    }
}