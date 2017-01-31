using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTOCtx")]
    public class VolTickOscCtx : IOneSourceHandler, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { set; private get; }

        public IList<double> Execute(ISecurity security)
        {
            var ctx = Context;
            var aCache = ctx.LoadObject("VTOCtx");
            var count = security.Bars.Count;
            var values = new double[count];
            var res = new double[count];
            if (aCache == null)
            {
                for (var i = 0; i < count; i++)
                {
                    var trades = security.GetTrades(i);
                    var valueTickBuy = 0.0;
                    var valueTickSell = 0.0;
                    var valueVolBuy = 0.0;
                    var valueVolSell = 0.0;

                    foreach (var t in trades)
                    {
                        var trd = t;
                        valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                        valueVolBuy += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                        valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                        valueVolSell += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                    }
                    // Считаем осциллятор

                    res[i] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                     (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                }
                ctx.StoreObject("VTOCtx", res);
                aCache = res;
            }
            
            
            else
            {
                var a = (double[])aCache;
                var b = a.Length;
                var d = b - count;
                for (var i = b; i < count; i++)
                {
                    var trades = security.GetTrades(i);
                    var valueTickBuy = 0.0;
                    var valueTickSell = 0.0;
                    var valueVolBuy = 0.0;
                    var valueVolSell = 0.0;

                    foreach (var t in trades)
                    {
                        var trd = t;
                        valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                        valueVolBuy += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                        valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                        valueVolSell += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                    }
                    // Считаем осциллятор

                    res[i] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                     (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                }

            }
            values = (double[])aCache;
            return values;
        }
    }
}