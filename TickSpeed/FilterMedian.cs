﻿using System.Collections.Generic;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
    [HandlerName("FilterMedian")]
    public class FilterMedianClass : IDouble2DoubleHandler
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