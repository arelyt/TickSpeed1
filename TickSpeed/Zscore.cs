using System.Collections.Generic;
using TSLab.Script.Handlers;
using Accord.Statistics;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Zscore")]
#pragma warning restore 612
    public class ZscoreClass : IDouble2DoubleHandler
    {
        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count, 1];
            var result = new double[count];
            //var valuesz = new double[count, 1];
            //
            for (var i = 0; i < count; i++)
            {
                values[i, 0] = myDoubles[i];
            }
            var valuesz = values.ZScores();

            for (int i = 0; i < count; i++)
            {
                result[i] = valuesz[i, 0];
            }
            return result;
        }
        
    }

}
