﻿using System;
using System.Collections.Generic;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Ускорение числа сделок на покупку/продажу в секунду в квадрате.
    [HandlerCategory("Arelyt"), HandlerName("TickAccel")]
    public class Taccel : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            var datme = new double[count];
            values[0] = 1;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var value = 0;

                datme[i] = TimeSpan.FromTicks(security.Bars[i].Date.Ticks - security.Bars[i - 1].Date.Ticks).TotalSeconds;

                for (var k = 0; k < trades.Count; k++)
                    value += trades[k].Direction == Direction ? 1 : 0;

                values[i] = value / datme[i] * datme[i];
            }
            return values;
        }
    }
}