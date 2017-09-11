using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;


namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Снижение значения N-баров подряд")]
#pragma warning restore 612
    public class NBarsDown : IDoubleCompaper1Handler
    {
        [HandlerParameter(true, "3", Max = "10", Min = "3", Step = "1")]
        public int NBars { get; set; }
        public IList<bool> Execute(IList<double> price)
        {
            if (NBars <= 0)
                NBars = 1;
            if (price == null || !price.Any())
            {
                return null;
            }
            bool[] flagArray = new bool[price.Count];
            int num = 0;
            for (int index = 0; index < price.Count; ++index)
            {
                flagArray[index] = false;
                if (index < NBars)
                    num = 0;
                else if (price[index] < price[index - 1])
                    ++num;
                else
                    num = 0;
                flagArray[index] = num >= NBars;
            }
            return flagArray;
        }

        
    }
}
