using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTOFilter")]
    public class VolTickOscFilterVol : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Size", NotOptimized = true)]
        public double Size { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy  = 0.0;
                var valueTickSell = 0.0;
                var valueVolBuy   = 0.0;
                var valueVolSell  = 0.0;

                foreach (var t in trades)
                {
                    var trd = t;
                    valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                    if (t.Direction.ToString() == "Buy" && trd.Quantity < Size)
                    {
                        valueVolBuy += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                    }
                    else
                    {
                        valueVolBuy += t.Direction.ToString() == "Buy" ? Size : 0;
                    }
                    
                    valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                    if (t.Direction.ToString() == "Sell" && trd.Quantity < Size)
                    {
                        valueVolSell += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                    }
                    else
                    {
                        valueVolSell += t.Direction.ToString() == "Sell" ? Size : 0;
                    }
                    
                }
                // Считаем осциллятор

                values[i] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                 (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
            }
            return values;
        }
    }
}