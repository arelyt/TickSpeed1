using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Уровни стакана")]
#pragma warning restore 612
    public class QueueLevels : IBar2DoubleHandler
    {
        private int _level;

        [HandlerParameter(Name = "Уровень стакана", Default = "1", NotOptimized = true)]
        public int Level
        {
            get => _level;
            set
            {
                if (value == 0)
                    throw new Exception("Уровень не может быть равен 0.");

                _level = value;
            }
        }

        public IList<double> Execute(ISecurity source)
        {
            var cnt = source.Bars.Count;
            if (cnt < 2)
                return null;

            // Стаканы на покупку и на продажу. Они раздельные.
            var buyQueue = source.GetBuyQueue(cnt - 1);
            var sellQueue = source.GetSellQueue(cnt - 1);

            // Подготовим массив для хранения выходных значений кубика.
            var values = new double[cnt];

            double resultVol = 0D;

            // Level не может быть равен 0, идет проверка в свойстве.
            // Поэтому используем такую конструкцию.
            if (Level < 0 && buyQueue.Count >= Math.Abs(Level))
            {
                // Если Level не больше чем строк в стакане, то вернем цену в нужной строке.
                // Иначе вернем просто 0.
                resultVol = buyQueue[-Level-1].Quantity;
            }

            if (Level > 0 && sellQueue.Count >= Math.Abs(Level))
                resultVol = sellQueue[Level - 1].Quantity;

            // Во все элементы результирующего массива кладем одну и ту же цену. 
            // Потому что истории стакана нет, можно получить только текущий стакан.
            for (var i = 0; i < cnt; i++)
                values[i] = resultVol;

            return values;
        }
    }
}
