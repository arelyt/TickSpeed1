using System;
using System.Collections.Generic;
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

        [HandlerParameter(Name = "Period", Default = "128", NotOptimized = true)]
        public int Step { get; set; }
        [HandlerParameter(Name = "Window", Default = "1024", NotOptimized = true)]
        public int Win { get; set; }
        [HandlerParameter(Name = "FilterTop", Default = "0", NotOptimized = true)]
        public int FilterTop { get; set; }
        [HandlerParameter(Name = "FilterBott", Default = "0", NotOptimized = true)]
        public int FilterBott { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            var ctx = Context;
            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");

         
            var count = ctx.BarsCount;
            var values = new double[count];
            var j = count-Win;
            for (var i = count-Win; i < count -1 ; i++)
            {
                double valueTickBuy = 0, valueTickSell = 0, valueVolBuy = 0, valueVolSell = 0;
                var t = sec.GetTradesPerBar(i-Step, i-1);
                
                foreach (var trades in t)
                {
                    valueTickBuy += trades[0].Direction.ToString() == "Buy" && trades[0].Quantity >= FilterBott && trades[0].Quantity < FilterTop ? 1 : 0;
                    valueVolBuy += trades[0].Direction.ToString() == "Buy" && trades[0].Quantity >= FilterBott && trades[0].Quantity < FilterTop ? trades[0].Quantity : 0;
                    valueTickSell += trades[0].Direction.ToString() == "Sell" && trades[0].Quantity >= FilterBott && trades[0].Quantity < FilterTop ? 1 : 0;
                    valueVolSell += trades[0].Direction.ToString() == "Sell" && trades[0].Quantity >= FilterBott && trades[0].Quantity < FilterTop ? trades[0].Quantity : 0;

                }

                //values[j] = (valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell) *
                //            (valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell);

                //values[j] = (valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                //            (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell);

                values[j] = ((valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell)) *
                            Math.Abs((valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell));
            
                j++;
            }
            return values;
        }
    }
}