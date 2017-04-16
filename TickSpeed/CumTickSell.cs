using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
    [HandlerName("CumTickSell")]
    public class CumTickSell : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;
            var valueTickSell = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                valueTickSell += trades.Sum(t => t.Direction.ToString() == "Sell" ? 1 : 0);
                values[i] = values[i - 1] + valueTickSell;
            }
            
            return values;
            

        }
    }
}

