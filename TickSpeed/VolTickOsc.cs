using System.Collections.Generic;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTO")]
    public class VolTickOsc : IBar2DoubleHandler
    {
        
        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                double valueTickBuy = trades.Sum(t => t.Direction.ToString() == "Buy" ? 1 : 0);
                double valueTickSell = trades.Sum(t => t.Direction.ToString() == "Sell" ? 1 : 0);
                var valueVolBuy = trades.Sum(t => t.Direction.ToString() == "Buy" ? t.Quantity : 0);
                var valueVolSell = trades.Sum(t => t.Direction.ToString() == "Sell" ? t.Quantity : 0);
                // Считаем осциллятор
                values[i] = ((valueTickBuy - valueTickSell)/(valueTickBuy + valueTickSell) +
                             (valueVolBuy - valueVolSell)/(valueVolBuy + valueVolSell))*100;
            }
            return values;
        }
    }
}