using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
    [HandlerName("CumDeltaTick")]
    public class CumDeltaTick : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;

            var values = new double[count];
            values[0] = 0;
            var valueTickBuy = 0;
            var valueTickSell = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);

                foreach (var t in trades)
                {
                    valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                    valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                }
                var cumtick = valueTickBuy - valueTickSell;
                values[i] = values[i - 1] + cumtick;
            }
            
            return values;
            

        }
    }
}

