﻿using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTO")]
    public class VolTickOsc : IBar2DoubleHandler
    {
        
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            if (count < 2)
                return null;


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

                values[i] = (valueVolBuy / valueTickBuy - valueVolSell / valueTickSell) /
                                 (valueVolBuy / valueTickBuy + valueVolSell / valueTickSell);
            }
            return values;
        }
    }
}