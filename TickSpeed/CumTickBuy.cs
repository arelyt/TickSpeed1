﻿using System.Collections.Generic;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumTickBuy")]
#pragma warning restore 612
    public class CumTickBuy : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count =security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;
            var valueTickBuy = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                valueTickBuy += trades.Sum(t => t.Direction.ToString() == "Buy" ? 1 : 0);
                values[i] = values[i - 1] + valueTickBuy;
            }
            
            return values;
            

        }
    }
}

