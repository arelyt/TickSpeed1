using System;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Options;

namespace TickSpeed.V2
{
    public class LoadTickGlobal : IExternalScript
    {
        public void Execute(IContext ctx, ISecurity sec)
        {

            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");

            var cache = ctx.LoadGlobalObject("TickPrice");

            var priceGlobal = (double[])cache;


            // вывод тиков инструмента на первую панель
            IGraphPane mainPane = ctx.CreateGraphPane("Главная", null);
            mainPane.Visible = true;
            mainPane.HideLegend = false;

            var color = new Color(System.Drawing.Color.Red.ToArgb());
            var lst = mainPane.AddList("Price", "tick", priceGlobal, ListStyles.POINT,  color, LineStyles.DOT, PaneSides.RIGHT);

                

        }
    }
}