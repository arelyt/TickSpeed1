using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("MoveVTO")]
#pragma warning restore 612
    public class MoveVtoGlobal : IOneSourceHandler, ISecurityInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { set; get ; }

        //[HandlerParameter(Name = "Period", Default = "16", NotOptimized = true)]
        //public int Step { get; set; }
        [HandlerParameter(Name = "Window", Default = "1024", NotOptimized = true)]
        public int Win { get; set; }
        [HandlerParameter(Name = "FilterTop", Default = "0", NotOptimized = true)]
        public int FilterTop { get; set; }
        [HandlerParameter(Name = "FilterBott", Default = "0", NotOptimized = true)]
        public int FilterBott { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            var ctx = Context;
            //if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
            //    throw new Exception("Base Interval wrong. Please set to Tick 1");

         
            var count = ctx.BarsCount;
            if (count < 2)
                return null;
            //var valuesvto = new double[count];
            var values = new double[count];
            var tBuy = new double[count];
            var tSell = new double[count];
            var vBuy = new double[count];
            var vSell = new double[count];

            for (int i = 0; i < Win-1; i++)
            {
                values[i] = 0;
            }
            for (var i = 0; i < count ; i++)
            {
                double valueTickBuy  = 0,
                       valueTickSell = 0,
                       valueVolBuy   = 0,
                       valueVolSell  = 0;
               
                    var t = sec.GetTrades(i);

                    foreach (var trades in t)
                    {
                        valueTickBuy += trades.Direction.ToString() == "Buy" &&
                            trades.Quantity >= FilterBott &&
                            trades.Quantity < FilterTop ? 1 : 0;
                        valueVolBuy += trades.Direction.ToString() == "Buy" &&
                            trades.Quantity >= FilterBott &&
                            trades.Quantity < FilterTop ? trades.Quantity : 0;
                        valueTickSell += trades.Direction.ToString() == "Sell" &&
                            trades.Quantity >= FilterBott &&
                            trades.Quantity < FilterTop ? 1 : 0;
                        valueVolSell += trades.Direction.ToString() == "Sell" &&
                            trades.Quantity >= FilterBott &&
                            trades.Quantity < FilterTop ? trades.Quantity : 0;

                    }


                tBuy[i] = valueTickBuy;
                tSell[i] = valueTickSell;
                vBuy[i] = valueVolBuy;
                vSell[i] = valueVolSell;

            }
            var tB = tBuy.Windowed(Win).Select(list => list.Sum()).ToList();
            var tS = tSell.Windowed(Win).Select(list => list.Sum()).ToList();
            var vB = vBuy.Windowed(Win).Select(list => list.Sum()).ToList();
            var vS = vSell.Windowed(Win).Select(list => list.Sum()).ToList();
            for (int k = Win-1; k < count; k++)
            {
               values[k] = (vB[k-Win+1]/tB[k - Win + 1] - vS[k-Win+1]/tS[k - Win + 1]) /
                           (vB[k-Win+1]/tB[k - Win + 1] + vS[k-Win+1]/tS[k - Win + 1]); 
            }
            return values;
        }
    }
}