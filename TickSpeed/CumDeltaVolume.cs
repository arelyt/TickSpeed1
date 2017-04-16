using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
    [HandlerName("CumDeltaVolume")]
    public class CumDeltaVolume : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var buyVolume = trades.Where(trd => trd.Direction == TradeDirection.Buy).Sum(trd => trd.Quantity);
                var sellVolume = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                var delta = buyVolume - sellVolume; // Просто дельта.
                values[i] = delta + values[i-1]; // Накопительная дельта.
            }
            return values;

        }
    }
}

