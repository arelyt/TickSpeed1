using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("VTOFull")]
    public class VolTickOscFull : IBar2DoubleHandler
    {
        public IContext Context { get; set; }

        [HandlerParameter(Name = "Шаг", NotOptimized = true)]
        public int Step { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            if (sec.IntervalBase.ToString() == "TICK" && sec.Interval.ToString() == "1")
            {
                // Главный цикл по тикам с шагом Step
                var tickcount = sec.Bars.Count;
                var values = new double[tickcount];
                for (var i = 0; i < tickcount; i += Step)
                {
                    // Проверка на последнюю свечу
                    if ((tickcount - Step*Convert.ToInt32(tickcount / Step) == 0 ))
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
                        for (var k = i; k < i + Step - 2; k++)
                        {
                            if (i == 0)
                            {
                                values[k] = 0.0;
                            }
                            else
                            {
                                values[k] = values[i + Step - 1];
                            }
                        }
                    }
                    else
                    {
                        // Итерационный цикл внутри последней свечи
                        var valueTickBuy = 0.0;
                        var valueTickSell = 0.0;
                        var valueVolBuy = 0.0;
                        var valueVolSell = 0.0;
                        var left = Convert.ToInt32(tickcount / Step);
                        for (var j = left; j <= tickcount; j++)
                        {
                            var t = sec.GetTrades(j);
                            valueTickBuy += t[0].Direction.ToString() == "Buy" ? 1 : 0;
                            valueVolBuy += t[0].Direction.ToString() == "Buy" ? t[0].Quantity : 0;
                            valueTickSell += t[0].Direction.ToString() == "Sell" ? 1 : 0;
                            valueVolSell += t[0].Direction.ToString() == "Sell" ? t[0].Quantity : 0;
                            // Считаем осциллятор
                        }
                        values[tickcount] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                             (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                        // Заполняем предшествующие элементы массива последним значением предыдущего шага
                        for (var k = left; k < tickcount; k++)
                        {
                            values[k] = values[left];
                        }
                    }
                }
                return values;
            }
            throw new Exception("Base Interval wrong. Please set to Tick 1");
        }
    }
}
