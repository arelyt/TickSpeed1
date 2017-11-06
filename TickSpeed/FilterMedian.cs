using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathNet.Filtering.Median;

namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("FilterMedian")]
#pragma warning restore 612
    public class FilterMedianClass : IDouble2DoubleHandler
    {
        [HandlerParameter(Name = "Window", Default = "31")]
        public int Win { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {

            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            //values[0] = 0;
            //for (var i = 0; i < count; i++)
            //{
            //    values[i] = myDoubles[i];
            //}

            var f = new OnlineMedianFilter(Win);
            for (var i = Win-1; i < count; i++)
            {
                values[i-Win+1] = f.ProcessSample(myDoubles[i]);
            }
            return values;
        }
        
    }

}
