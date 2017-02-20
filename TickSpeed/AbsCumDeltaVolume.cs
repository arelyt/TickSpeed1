using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор абсолютной накопительной дельты объема.
    [HandlerCategory("Arelyt")]
    [HandlerName("AbsCumDeltaVolume")]
    public class AbsCumDeltaVolumeClass : IBar2DoubleHandler
    {
        //[HandlerParameter(Name = "Window", NotOptimized = true)]
        //public int win { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            values[0] = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var buyVolume = trades.Where(trd => trd.Direction == TradeDirection.Buy).Sum(trd => trd.Quantity);
                var sellVolume = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                var delta = buyVolume - sellVolume; // Просто дельта.
                values[i] = Math.Abs(delta) + values[i-1]; // Абсолютная накопительная дельта.
            }
            return values;

        }
    }
}

