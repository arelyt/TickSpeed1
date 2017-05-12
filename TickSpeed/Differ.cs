using System.Collections.Generic;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
    [HandlerName("Diff")]
    public class DifferClass : IDouble2DoubleHandler
    {
        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;
            for (var i = 1; i < count; i++)
            {
                values[i] = myDoubles[i] - myDoubles[i - 1];
            }
            return values;
        }
        
    }

}
