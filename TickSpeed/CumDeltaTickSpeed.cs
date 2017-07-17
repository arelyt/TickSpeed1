using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индикатор накопительной дельты объема по Шибаеву :-).
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumDeltaTickSpeed")]
#pragma warning restore 612
    public class CumDeltaTickSpeed : IBar2DoubleHandler
    {
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;

            var values = new double[count];
            var datme = new double[count];
            values[0] = 0;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);
                //var valueTickBuy = trades.Count(trd => trd.Direction == TradeDirection.Buy);
                //var valueTickSell = trades.Count(trd => trd.Direction == TradeDirection.Sell);
                var nBuy = trades.Sum(t => t.Direction == TradeDirection.Buy ? 1 : 0); ;
                var nSell = trades.Sum(t => t.Direction == TradeDirection.Sell ? 1 : 0); ;

                datme[i] = TimeSpan.FromTicks(security.Bars[i].Date.Ticks - security.Bars[i - 1].Date.Ticks).TotalSeconds;
                if (datme[i] < 0.0001)
                {
                    datme[i] = 0.00001;
                }
                //var cumtickspeed = (valueTickBuy - valueTickSell)/datme[i];
                //values[i] = values[i - 1] + cumtickspeed;
                values[i] = (values[i-1] + (nBuy-nSell)/datme[i];
            }
            
            return values;
            

        }
    }
}

