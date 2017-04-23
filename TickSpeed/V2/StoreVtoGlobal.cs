using System;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed.V2
{
    public class StoreVtoGlobal : IExternalScript
    {
        public int Step = 8;
        public void Execute(IContext ctx, ISecurity sec)
        {

            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");


            double[] vto8 = vto_step(sec, ctx, Step);


            //var cache = ctx.LoadGlobalObject("TickPrice");
            //var price = new double[ctx.BarsCount];
            //for (var i = 0; i < ctx.BarsCount; i++)
            //{
            //   //var t = sec.GetTrades(i);
            //    price[i] = sec.GetTrades(i)[0].Price;
            //}
            
            ctx.StoreGlobalObject("VTO8", vto8);    

        }
        public double[] vto_step(ISecurity scr, IContext ctx, int in1)
        {
            var count = Convert.ToInt32(ctx.BarsCount / in1);
            var values = new double[count];
            for (var i = 0; i < count - 1; i += in1)
            {
                double valueTickBuy = 0, valueTickSell = 0, valueVolBuy = 0, valueVolSell = 0;
                var t = scr.GetTradesPerBar(i, i + in1 - 1);
                foreach (var trades in t)
                {
                    valueTickBuy += trades[0].Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += trades[0].Direction.ToString() == "Buy" ? trades[0].Quantity : 0;
                    valueTickSell += trades[0].Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += trades[0].Direction.ToString() == "Sell" ? trades[0].Quantity : 0;

                }
                values[i] = (valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell) *
                            (valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell);
            }
            return values;
        }
    }
}