using System.Collections.Generic;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt", "Arelyt")]
    [HandlerName("Diff")]
    [InputsCount(1)]
    public class DifferClass : IDouble2DoubleHandler, IStreamHandler
    {
        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
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
