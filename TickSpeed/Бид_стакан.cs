using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
namespace TickSpeed
{
#pragma warning disable 612
    [HandlerCategory("Arelyt")]
    [HandlerName("Стакан_бид")]
#pragma warning restore 612
    public class Bid : ISecurityInputs, IDoubleReturns, IOneSourceHandler, IStreamHandler
    {
        public IList<double> Execute(ISecurity source)
        {
            var numArray = new double[source.Bars.Count];
            for (var i = 0; i < source.Bars.Count; i++)
            {
                var buyQueue = source.GetBuyQueue(i);
                numArray[i] = (buyQueue.Count > Level) ? buyQueue[Level].Price : 0.0;
            }
            return numArray;
        }

        [HandlerParameter(true, "0", Min="1", Max="20", Step="1")]
        public int Level { get; set; }
    }
}

