using System;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Options;

namespace TickSpeed.V2
{
    public class StoreTickGlobal : IExternalScript
    {
        public void Execute(IContext ctx, ISecurity sec)
        {

            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");

            //var cache = ctx.LoadGlobalObject("TickPrice");
            var price = new double[ctx.BarsCount];
            for (var i = 0; i < ctx.BarsCount; i++)
            {
               //var t = sec.GetTrades(i);
                price[i] = sec.GetTrades(i)[0].Price;
            }
            
            ctx.StoreGlobalObject("TickPrice", price);    

        }
    }
}