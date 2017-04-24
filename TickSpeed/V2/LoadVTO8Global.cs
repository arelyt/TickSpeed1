using System;
using System.Collections.Generic;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Options;

namespace TickSpeed.V2
{
    public class LoadVto8Global : IExternalScript
    {
        public int Step = 8;
        public void Execute(IContext ctx, ISecurity sec)
        {
            
            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");

            var cache = ctx.LoadGlobalObject("VTO8");

            var vtoGlobal = (double[])cache;
            Interval interval = new Interval(Step, DataIntervals.TICK);
            var comp = sec.CompressTo(interval);
            //var comp = sec.CompressTo(new Interval(Step * sec.Interval, sec.IntervalBase));
            var vto8 = comp.Decompress(vtoGlobal);


            // вывод тиков инструмента на первую панель
            IGraphPane mainPane = ctx.CreateGraphPane("Главная", null);
            mainPane.Visible = true;
            mainPane.HideLegend = false;
            

            var color = new Color(System.Drawing.Color.Blue.ToArgb());
            var lst = mainPane.AddList("VTO", "vto8", vto8, ListStyles.LINE,  color, LineStyles.SOLID, PaneSides.LEFT);
            var color1 = new Color(System.Drawing.Color.Green.ToArgb());
            mainPane.AddList("Tick", "tt", sec, CandleStyles.CANDLE_AND_QUEUE, color1, PaneSides.RIGHT);
            mainPane.UpdatePrecision(PaneSides.LEFT, 2);


        }
    }
}