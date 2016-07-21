using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
    [HandlerName("DeltaVolume")]
    public class DeltaVolume : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var buyVolume = trades.Where(trd => trd.Direction == TradeDirection.Buy).Sum(trd => trd.Quantity);
                var sellVolume = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                var delta = buyVolume-sellVolume; // Просто дельта.
                values[i] = delta;
            }
            return values;

        }
    }
}
    
