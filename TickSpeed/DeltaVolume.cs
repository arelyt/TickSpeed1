using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор дельты объема:-).
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("DeltaVolume")]
#pragma warning restore 612
    public class DeltaVolume : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
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
    
