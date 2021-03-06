﻿using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // Скорость сделок на покупку/продажу в секунду.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("TSpeed")]
#pragma warning restore 612
    public class TspeedClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        [HandlerParameter(Name = "Win", NotOptimized = true)]
        public double win { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            if (count < 2)
                return null;

            var datme = new double[count];
            values[0] = 1;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);

            datme[i] = TimeSpan.FromMilliseconds(security.Bars[i].Date.TimeOfDay.TotalMilliseconds - security.Bars[i-1].Date.TimeOfDay.TotalMilliseconds).TotalSeconds;
                //datme[i] = security.Bars[i].Date.TimeOfDay.TotalMilliseconds;
                var value = trades.Sum(t => t.Direction == Direction ? 1 : 0);

                //  Проверка на ненулевое время (м.б. ошибка в тиковых данных или их отсутствие. Принудительно делим на 0.1)
                if (datme[i] > 0.0001)
                    values[i] = value / datme[i];
                else
                    values[i] = value / 1.0;
                
                
            }
            values = (double[]) Series.EMA(values, (int)win);
            return values;
        }
    }
}