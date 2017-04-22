using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной суммы объема продаж.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumVolumeSell")]
#pragma warning restore 612
    public class CumVolumeSell : IBar2DoubleHandler
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
                var sellVolume = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                values[i] = sellVolume + values[i-1]; // Накопительная сумма продаж.
            }
            return values;

        }
    }
}

