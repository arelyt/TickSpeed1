using System.Collections.Generic;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;


namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Рост значения N-баров подряд")]
#pragma warning restore 612
    public class NBarsUp : ITwoSourcesHandler, ISecurityInput0, IDoubleInput1, IStreamHandler, IBooleanReturns
    {
        [HandlerParameter(true, "3", Max = "10", Min = "3", Step = "1")]
        public int NBars { get; set; }
        public IList<bool> Execute(ISecurity sec, IList<double> price)
        {
            if (NBars <= 0)
                NBars = 1;
            bool[] flagArray = new bool[sec.Bars.Count];
            for (int i = 0; i < sec.Bars.Count; i++)
            {
                flagArray[i] = false;
            }
            if (price == null || !price.Any())
            {

                return flagArray;
            }
            //bool[] flagArray = new bool[price.Count];
            int num = 0;
            for (int index = 0; index < price.Count; ++index)
            {
                flagArray[index] = false;
                if (index < NBars)
                    num = 0;
                else if (price[index] > price[index - 1])
                    ++num;
                else
                    num = 0;
                flagArray[index] = num >= NBars;
            }
            return flagArray;
        }

        
    }
}
