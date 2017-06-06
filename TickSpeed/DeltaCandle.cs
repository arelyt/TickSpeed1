using System;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    /* 
    Аналог сжатия ТСлаба, но по шагам дельты для дальнейшей возможной группировки в зависимости от частотного анализа рынка.
    */

    [HandlerCategory("Arelyt")]
    [HandlerName("Delta Candles")]
    public class DeltaCandle : IOneSourceHandler, ISecurityInputs, ISecurityReturns, IStreamHandler, IContextUses
    {
        public IContext Context { set; get; }

        [HandlerParameter(Name = "Ширина свечи(шагов дельты)", Default = "500", Min = "50", Max = "2000", Step = "10")]
        public int Step { get; set; }

        //        [HandlerParameter(Name = "Полный расчет", Default = "false", NotOptimized = true)]
        //        public bool CalcFullCandle { get; set; }

        public ISecurity Execute(ISecurity sec, IContext context)
        {
            if (sec.IntervalBase.ToString() != "TICKS" || sec.Interval.ToString() != "1")
            {
                throw new Exception("Base Interval wrong. Please set to Ticks 1");
            }

            //var cache = ctx.LoadObject("TickCandle");
            //if (cache==null)
            //{


            //} 
            // Главный цикл по тикам с шагом Step
            var tickcount = context.BarsCount;
            if (tickcount < 2)
                return null;
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
