using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("MovCumVTO")]
#pragma warning restore 612
    public class MovCumVtoClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Window", Default = "16", Min = "2", Max = "64", Step = "1")]
        public int Step { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            var cumTickBuy = new double[count];
            var cumTickSell = new double[count];
            var cumVolBuy = new double[count];
            var cumVolSell = new double[count];

            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy = trades.Count(trd => trd.Direction == TradeDirection.Buy);
                var valueVolBuy = trades.Where(trd => trd.Direction == TradeDirection.Buy).Sum(trd => trd.Quantity);
                var valueTickSell = trades.Count(trd => trd.Direction == TradeDirection.Sell);
                var valueVolSell = trades.Where(trd => trd.Direction == TradeDirection.Sell).Sum(trd => trd.Quantity);
                cumTickBuy[i] = cumTickBuy[i] + valueTickBuy;
                cumTickSell[i] = cumTickSell[i] + valueTickSell;
                cumVolBuy[i] = cumVolBuy[i] + valueVolBuy;
                cumVolSell[i] = cumVolSell[i] + valueVolSell;
                // Считаем осциллятор
                
            }
            for (int i = 0; i < count; i++)
            {
                if (i < Step)
                    values[i] = 0;
                else
                {
                    values[i] = ((cumVolBuy[i] - cumTickBuy[i-Step])/(cumTickBuy[i] - cumTickBuy[i - Step]) -
                                 (cumVolSell[i]-cumVolSell[i-Step])/(cumTickSell[i] - cumTickSell[Step])) /
                                 ((cumVolBuy[i] - cumTickBuy[i - Step])/(cumTickBuy[i] - cumTickBuy[i - Step]) +
                                 (cumVolSell[i] - cumVolSell[i - Step])/(cumTickSell[i] - cumTickSell[Step]));
                }
            }
            return values;
        }
    }
}