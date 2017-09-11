using System.Collections.Generic;
using RusAlgo.Helper;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
    [HandlerName("Рост значения N-баров подряд")]
    public class NBarsGrouth : IDoubleCompaper1Handler
    {
        [HandlerParameter(true, "3", Max = "10", Min = "3", Step = "1")]
        public int NBars { get; set; }
        public IList<bool> Execute(IList<double> price)
        {
            if (this.NBars <= 0)
                this.NBars = 1;
            if (price[0].IsNaN())
            {
                return null;
            }
            bool[] flagArray = new bool[price.Count];
            int num = 0;
            for (int index = 0; index < price.Count; ++index)
            {
                flagArray[index] = false;
                if (index < this.NBars)
                    num = 0;
                else if (price[index] > price[index - 1])
                    ++num;
                else
                    num = 0;
                flagArray[index] = num >= this.NBars;
            }
            return (IList<bool>)flagArray;
        }

        
    }
}
