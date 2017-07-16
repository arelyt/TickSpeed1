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
        [HandlerParameter(Name = "Объем_тики", Default = "true", NotOptimized = true)]
        public bool Method { get; set; }
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
                if (Method)
                {
                    var vBuy = trades.Sum(t => t.Direction == TradeDirection.Buy ? t.Quantity : 0); ;
                    var vSell = trades.Sum(t => t.Direction == TradeDirection.Sell ? t.Quantity : 0); ;
                    _cumdelta += (int)vBuy - (int)vSell;
                }
                else
                {
                    var nBuy = trades.Sum(t => t.Direction == TradeDirection.Buy ? 1 : 0); ;
                    var nSell = trades.Sum(t => t.Direction == TradeDirection.Sell ? 1 : 0); ;
                    _cumdelta += nBuy - nSell;
                }
                


                if (_cumdelta >= _indi + Step || _cumdelta <= _indi - Step)
                {
                    values[i] = _cumdelta;
                    _indi = _cumdelta;
                }

                
                
            }
            values[count - 1] = _cumdelta;
            return values;
        }
    }
}