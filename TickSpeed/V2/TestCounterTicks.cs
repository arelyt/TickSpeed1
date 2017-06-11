using RusAlgo.Helper;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed.V2
{
    public class TestCounterTicksClass : IExternalScript
    {
        public static double Counter;
        public void Execute(IContext ctx, ISecurity sec)
        {
            
            //if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
            //    throw new Exception("Base Interval wrong. Please set to Tick 1");
            Counter++;
            //var cache = ctx.LoadGlobalObject("TickPrice");
            //var price = new double[ctx.BarsCount];
            //for (var i = 0; i < ctx.BarsCount; i++)
            //{
            //   //var t = sec.GetTrades(i);
            //    price[i] = sec.GetTrades(i)[0].Price;
            //}
            
            //ctx.StoreGlobalObject("TickPrice", price);    
            ctx.Log("Counter {0}".Put(Counter), MessageType.Info, true);
        }
    }
}