using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{

    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Стакан_Аск_Объем")]
#pragma warning restore 612
    public class AskQty : ISecurityInputs, IDoubleReturns, IOneSourceHandler, IStreamHandler
    {
        public IList<double> Execute(ISecurity source)
        {
            var numArray = new double[source.Bars.Count];
            for (var i = 0; i < source.Bars.Count; i++)
            {
                var sellQueue = source.GetSellQueue(i);
                numArray[i] = (sellQueue.Count > Level) ? sellQueue[Level].Quantity : 0.0;
            }
            return numArray;
        }

        [HandlerParameter(true, "0", Min="1", Max="20", Step="1")]
        public int Level { get; set; }
    }
}

