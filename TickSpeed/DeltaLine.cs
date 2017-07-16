using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Скорость сделок на покупку/продажу в секунду.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("DeltaLine")]
#pragma warning restore 612
    public class DeltaLineClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        [HandlerParameter(Name = "Шаг дельты", Default = "50", Min = "2", Max = "2000", Step = "1")]
        public int Step { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            if (count < 2)
                return null;

            var _cumdelta = 0;
            values[0] = 1;
            var _indi = 0;
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var nBuy = trades.Sum(t => t.Direction == TradeDirection.Buy ? 1 : 0); ;
                var nSell = trades.Sum(t => t.Direction == TradeDirection.Sell ? 1 : 0); ;
                _cumdelta += nBuy - nSell; 


                if (_cumdelta >= _indi + Step || _cumdelta <= _indi - Step)
                {
                    values[i] = _cumdelta;
                    _indi += Step;
                }

                //var value = trades.Sum(t => t.Direction == Direction ? 1 : 0);

                //  Проверка на ненулевое время (м.б. ошибка в тиковых данных или их отсутствие. Принудительно делим на 0.1)
                //if (datme[i] > 0.0001)
                //    values[i] = value / datme[i];
                //else
                //    values[i] = value / 0.1;
                
                
            }
            values[count - 1] = _cumdelta;
            return values;
        }
    }
}