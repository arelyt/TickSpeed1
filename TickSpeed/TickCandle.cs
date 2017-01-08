using System;
using System.Collections.Generic;
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
        public IContext Context { set; get; }

        [HandlerParameter(Name = "Ширина свечи(тиков)", Default = "16", Min = "2", Max = "64", Step = "1")]
        public int Step { get; set; }

        //        [HandlerParameter(Name = "Полный расчет", Default = "false", NotOptimized = true)]
        //        public bool CalcFullCandle { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");
            // Главный цикл по тикам с шагом Step
            var tickcount = sec.Bars.Count;
            var values = new double[tickcount];
            for (var i = 0; i < tickcount; i += Step)
            {
                // Проверка на последнюю свечу
                if ((tickcount - Step * Convert.ToInt32(tickcount / Step) == 0))
                {
                    // Итерационный цикл внутри выбранного периода

                    var valueTickBuy = 0.0;
                    var valueTickSell = 0.0;
                    var valueVolBuy = 0.0;
                    var valueVolSell = 0.0;
                    for (var j = i; j < i + Step; j++)
                    {
                        var t = sec.GetTrades(j);
                        valueTickBuy += t[0].Direction.ToString() == "Buy" ? 1 : 0;
                        valueVolBuy += t[0].Direction.ToString() == "Buy" ? t[0].Quantity : 0;
                        valueTickSell += t[0].Direction.ToString() == "Sell" ? 1 : 0;
                        valueVolSell += t[0].Direction.ToString() == "Sell" ? t[0].Quantity : 0;
                        // Считаем осциллятор
                    }
                    values[i + Step - 1] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                            (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                    // Заполняем предшествующие элементы массива последним значением предыдущего шага
                    //for (var k = i; k < i + Step - 2; k++)
                    //{
                    //    if (i == 0)
                    //    {
                    //        values[k] = 0.0;
                    //    }
                    //    else
                    //    {
                    //        values[k] = values[i + Step - 1];
                    //    }
                    //}
                }
            }
            var comp = sec.CompressTo(new Interval(Step*sec.Interval, sec.IntervalBase));
            var vtoBars = new DataBar[comp.Bars.Count];
            for (int k = 0; k < comp.Bars.Count; k++)
            {
                var open = comp.Bars[k].Open;
                var close = comp.Bars[k].Close;
                var high = comp.Bars[k].High;
                var low = comp.Bars[k].Low;
                var date = comp.Bars[k].Date;
                var bar = new DataBar(date, open, high, low, close, 10000*values[(k+1) * Step - 1], values[(k + 1) * Step - 1]);
                vtoBars[k] = bar;
            }
            var vto = comp.CloneAndReplaceBars(vtoBars);
            return vto;
        }
    }
}
