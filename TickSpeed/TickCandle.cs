﻿using System;
using System.Linq;
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

        [HandlerParameter(Name = "Ширина свечи(тиков)", Default = "16", Min = "2", Max = "64", Step = "1")]
        public int CandleWidth { get; set; }

        //        [HandlerParameter(Name = "Полный расчет", Default = "false", NotOptimized = true)]
        //        public bool CalcFullCandle { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            if (sec.IntervalBase.ToString() == "TICK" && sec.Interval.ToString() == "1")

            {
                var tickcandlecount = Context.BarsCount;
                var newcandlecount = Convert.ToInt32(tickcandlecount / CandleWidth);
                //                var vto = new double[newcandlecount];
                var volBars = new DataBar[newcandlecount];
                volBars[0] = new DataBar(sec.Bars[0].Date, sec.Bars[1].Open, sec.Bars[1].High,
                    sec.Bars[1].Low, sec.Bars[1].Close, sec.Bars[1].Volume);

                for (var i = 1; i < tickcandlecount; i += CandleWidth)
                {
                    var price = new double[CandleWidth];
                    var buycount = 0;
                    var sellcount = 0;
                    double volbuy = 0;
                    double volsell = 0;
                    double vto = 0;

                    for (var j = 0; j < CandleWidth; j++)
                    {
                        price[j] = sec.GetTrades(i + j).First().Price;

                        if (sec.GetTrades(i + j).First().Direction.ToString() == "Buy")
                        {
                            buycount++;
                            volbuy = volbuy + sec.GetTrades(i + j).First().Volume;
                        }
                        else
                        {
                            sellcount++;
                            volsell = volsell + sec.GetTrades(i + j).First().Volume;
                        }

                        vto = (buycount * volbuy - sellcount * volsell) / (buycount * volbuy + sellcount * volsell);
                    }
                    var date = sec.Bars[i].Date;
                    var volOpen = sec.Bars[i].Open;
                    var volClose = sec.Bars[i + CandleWidth - 1].Open;

                    //                    var volHigh = Math.Max(volOpen, volClose);
                    //                    var volLow = Math.Min(volOpen, volClose);

                    // если нужен расчет с учетом теней и фактических сделок
                    //if (CalcFullCandle)
                    //{
                    //    var ticks = sec.GetTrades(i);
                    //    var tt = ticks[i].Direction;
                    //    if (ticks.AnyNotNull())
                    //    {
                    //        volOpen = ticks.First().OpenInterest;
                    //        volClose = ticks.Last().OpenInterest;
                    //        volHigh = ticks.Max(t => t.OpenInterest);
                    //        volLow = ticks.Min(t => t.OpenInterest);
                    //    }
                    //}

                    //var volVolume = Math.Abs(volClose - volOpen);

                    var bar = new DataBar(date, volOpen, volOpen, volClose, volClose, vto);
                    volBars[i] = bar;
                }

                // клонируем с подменой баров, получаем типо инструмент, но свечи иные.
                var volSec = sec.CloneAndReplaceBars(volBars);
                return volSec;
            }
            throw new Exception("Base Interval wrong. Please set to Tick 1");
        }
    }
}
