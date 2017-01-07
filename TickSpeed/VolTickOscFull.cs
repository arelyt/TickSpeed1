using System.Collections.Generic;
using System.Linq;
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

            // Главный цикл по тикам с шагом Step
            var tickcount = sec.Bars.Count;
            var values = new double[tickcount];
            for (var i = 0; i < tickcount; i += Step)
            {
                // Проверка на последнюю свечу
                if ((i*Step) < tickcount)
                {

                    // Итерационный цикл внутри выбранного периода
                    var candlevto = 0.0;
                    var valueTickBuy = 0.0;
                    var valueTickSell = 0.0;
                    var valueVolBuy = 0.0;
                    var valueVolSell = 0.0;
                    for (var j = i; j < i + Step; j++)
                    {
                        var t = sec.GetTrades(j);
                        valueTickBuy += t.First().Direction.ToString() == "Buy" ? 1 : 0;
                        valueVolBuy += t.First().Direction.ToString() == "Buy" ? t.First().Volume : 0;
                        valueTickSell += t.First().Direction.ToString() == "Sell" ? 1 : 0;
                        valueVolSell += t.First().Direction.ToString() == "Sell" ? t.First().Volume : 0;
                        // Считаем осциллятор

                    }
                    candlevto = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                 (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                    values[i + Step] = candlevto;
                    // Заполняем предшествующие элементы массива последним значением предыдущего шага
                    for (int k = i; k < i + Step - 1; k++)
                    {
                        if (i == 0)
                        {
                            values[k] = 0.0;
                        }
                        else
                        {
                            values[k] = values[i];
                        }

                    }
                }
                else
                {
                    // Итерационный цикл внутри последней свечи
                    var candlevto = 0.0;
                    var valueTickBuy = 0.0;
                    var valueTickSell = 0.0;
                    var valueVolBuy = 0.0;
                    var valueVolSell = 0.0;
                    for (var j = i; j <= tickcount; j++)
                    {
                        var t = sec.GetTrades(j);
                        valueTickBuy += t.First().Direction.ToString() == "Buy" ? 1 : 0;
                        valueVolBuy += t.First().Direction.ToString() == "Buy" ? t.First().Volume : 0;
                        valueTickSell += t.First().Direction.ToString() == "Sell" ? 1 : 0;
                        valueVolSell += t.First().Direction.ToString() == "Sell" ? t.First().Volume : 0;
                        // Считаем осциллятор

                    }
                    candlevto = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                 (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
                    values[tickcount] = candlevto;
                    // Заполняем предшествующие элементы массива последним значением предыдущего шага
                    for (var k = i; k <= tickcount-1; k++)
                    {
                        values[k] = values[i];
                    }
                }
            }
            return values;

        }
            
    }

}
