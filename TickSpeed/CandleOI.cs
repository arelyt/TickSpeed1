using System;
using System.Linq;
using RusAlgo.Helper;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Индюкатор выдает ОИ инструмента как сам инструмент. В итоге видим свечки на графике по ОИ и объемы
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("OI Candles")]
#pragma warning restore 612
    public class CandleOi : IOneSourceHandler, ISecurityInputs, ISecurityReturns, IStreamHandler, IContextUses
    {
        public IContext Context { set; private get; }

        [HandlerParameter(Name = "Полный расчет", Default = "false", NotOptimized = true)]
        public bool CalcFullCandle { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            var count = sec.Bars.Count;
            if (count < 2)
                return null;
            var oiBars = new DataBar[Context.BarsCount];
            oiBars[0] = new DataBar(sec.Bars[0].Date, sec.Bars[1].Interest, sec.Bars[1].Interest, sec.Bars[1].Interest, sec.Bars[1].Interest);
            for (int i = 1; i < Context.BarsCount; i++)
            {
                
                var date = sec.Bars[i].Date;
                var oiOpen = sec.Bars[i - 1].Interest;
                var oiClose = sec.Bars[i].Interest;

                var oiHigh = Math.Max(oiOpen, oiClose);
                var oiLow = Math.Min(oiOpen, oiClose);

                // если нужен расчет ОИ с учетом теней и фактических сделок
                if (CalcFullCandle)
                {
                    var ticks = sec.GetTrades(i);

                    if (ticks.AnyNotNull())
                    {
                        oiOpen = ticks.First().OpenInterest;
                        oiClose = ticks.Last().OpenInterest;
                        oiHigh = ticks.Max(t => t.OpenInterest);
                        oiLow = ticks.Min(t => t.OpenInterest);
                    }
                }

                var oiVolume = Math.Abs(oiClose - oiOpen);

                var bar = new DataBar(date, oiOpen, oiHigh, oiLow, oiClose, oiVolume);

                oiBars[i] = bar;
            }

            // клонируем с подменой баров, получаем типо инструмент, но свечи иные.
            var oiSec = sec.CloneAndReplaceBars(oiBars);
            return oiSec;
        }
    }
}
