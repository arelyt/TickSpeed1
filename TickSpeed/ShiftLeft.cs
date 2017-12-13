using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
    [HandlerName("ShiftLeft")]
    public class ShiftLeftClass : IDouble2DoubleHandler
    {
        [HandlerParameter(true, "0", Name = "LShift", Max = "20", Min = "0", Step = "1", NotOptimized = false)]
        public int ShiftL { get; set; }
        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            //var values = new double[count];
            //values[0] = 0;
            //for (var i = 1; i < count; i++)
            //{
            //    values[i] = myDoubles[i] - myDoubles[i - 1];
            //}
            var result = myDoubles.TakeLast(count - ShiftL);
            return result.ToList();
        }
        
    }

}
