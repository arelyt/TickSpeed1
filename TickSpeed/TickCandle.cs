using System;
using System.Linq;
using RusAlgo.Helper;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    /* Индикатор выдает сжатые на базе кратных 10 тикам
    свечи инструмента как сам инструмент.
    В итоге видим обыкновенные свечки на графике.
    Аналог сжатия ТСлаба, но с сохранением суммированных
    тиковых значений для дальнейшей возможной группировки в зависимости от частотного анализа рынка.
    */

    [HandlerCategory("Arelyt")]
    [HandlerName("Tick Candles")]
    public class TickCandle : IOneSourceHandler, ISecurityInputs, ISecurityReturns, IStreamHandler, IContextUses
    {
        public IContext Context { set; private get; }

        [HandlerParameter(Name = "Полный расчет", Default = "false", NotOptimized = true)]
        public bool CalcFullCandle { get; set; }
        public TradeDirection Direction { get; set; }
        public ISecurity Execute(ISecurity sec)
        {
            if (sec.IntervalBase.ToString() == "TICK" && sec.Interval.ToString() == "10")
                throw new Exception("Base Interval wrong. Please set to Tick 10");
            {
                var volBars = new DataBar[Context.BarsCount];
                volBars[0] = new DataBar(sec.Bars[0].Date, sec.Bars[1].Volume, sec.Bars[1].Volume,
                    sec.Bars[1].Volume, sec.Bars[1].Volume);
                for (int i = 1; i < Context.BarsCount; i++)
                {

                    var date = sec.Bars[i].Date;
                    var volOpen = sec.Bars[i - 1].Volume;
                    var volClose = sec.Bars[i].Volume;

                    var volHigh = Math.Max(volOpen, volClose);
                    var volLow = Math.Min(volOpen, volClose);

                    // если нужен расчет с учетом теней и фактических сделок
                    if (CalcFullCandle)
                    {
                        var ticks = sec.GetTrades(i);
                        var tt = ticks[i].Direction;
                        if (ticks.AnyNotNull())
                        {
                            volOpen = ticks.First().OpenInterest;
                            volClose = ticks.Last().OpenInterest;
                            volHigh = ticks.Max(t => t.OpenInterest);
                            volLow = ticks.Min(t => t.OpenInterest);
                        }
                    }

                    var volVolume = Math.Abs(volClose - volOpen);

                    var bar = new DataBar(date, volOpen, volHigh, volLow, volClose, volVolume);

                    volBars[i] = bar;
                }

                // клонируем с подменой баров, получаем типо инструмент, но свечи иные.
                var volSec = sec.CloneAndReplaceBars(volBars);
                return volSec;
            }
        }
    }
}
