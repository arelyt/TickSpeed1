﻿using System;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed.V2
{
    public class StoreMoveVtoGlobal : IExternalScript
    {
        public int Step = 128;
        public int Win = 1024;
        public void Execute(IContext ctx, ISecurity sec)
        {

            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");


            double[] vto16 = vto_step(sec, ctx, Step);


            //var cache = ctx.LoadGlobalObject("TickPrice");
            //var price = new double[ctx.BarsCount];
            //for (var i = 0; i < ctx.BarsCount; i++)
            //{
            //   //var t = sec.GetTrades(i);
            //    price[i] = sec.GetTrades(i)[0].Price;
            //}
            
            ctx.StoreGlobalObject("VTO16", vto16);

            // вывод тиков инструмента на первую панель
            IGraphPane mainPane = ctx.CreateGraphPane("Главная", null);
            mainPane.Visible = true;
            mainPane.HideLegend = false;


            var color = new Color(System.Drawing.Color.Blue.ToArgb());
            var lst = mainPane.AddList("VTO", "vto16", vto16, ListStyles.LINE, color, LineStyles.SOLID, PaneSides.LEFT);
            var color1 = new Color(System.Drawing.Color.Green.ToArgb());
            mainPane.AddList("Tick", "tt", sec, CandleStyles.CANDLE_AND_QUEUE, color1, PaneSides.RIGHT);
            mainPane.UpdatePrecision(PaneSides.LEFT, 2);

        }
        public double[] vto_step(ISecurity scr, IContext ctx, int in1)
        {
            var count = ctx.BarsCount;
            var values = new double[count];
            var j = count-Win;
            for (var i = count-Win; i < count -1 ; i++)
            {
                double valueTickBuy = 0, valueTickSell = 0, valueVolBuy = 0, valueVolSell = 0;
                var t = scr.GetTradesPerBar(i-Step, i-1);
                
                foreach (var trades in t)
                {
                    valueTickBuy += trades[0].Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += trades[0].Direction.ToString() == "Buy" ? trades[0].Quantity : 0;
                    valueTickSell += trades[0].Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += trades[0].Direction.ToString() == "Sell" ? trades[0].Quantity : 0;

                }

                //values[j] = (valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell) *
                //            (valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell);

                values[j] = (valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                            (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell);

                //values[j] = ((valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell)) *
                //           Math.Abs((valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell));
            
                j++;
            }
            return values;
        }
    }
}