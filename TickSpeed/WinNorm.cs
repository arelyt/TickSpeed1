using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Скользящее нормализованное окно накопительной дельты
#pragma warning disable 612
    [HandlerCategory("Arelyt")]
    [HandlerName("WinNorm")]
#pragma warning restore 612
    public class WinNormClass : IDouble2DoubleHandler
    {
        [HandlerParameter(true, "64", Name = "Win")]
        public int Win { get; set; }

        [HandlerParameter(true, "0.5", Name = "K")]
        public double K { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count]; // values result
            for (int i = 0; i < Win - 1; i++)
            {
                values[i] = 0.0;//myDoubles[i];
            }
            for (int i = Win - 1; i < count; i++)
            {
                var start = Math.Max(i - Win + 1, 0);
                var w = myDoubles.Skip(start).Take(Win).ToArray();
                //var bs = (w.Max() + w.Min()) / 2;

                //values[i] = (Math.Exp(K * (myDoubles[i] - bs)) - 1) /
                //            (Math.Exp(K * (myDoubles[i] - bs)) + 1);
                values[i] = 2.0*(myDoubles[i] - w.Min())/(w.Max() - w.Min()) - 1.0;

            }
            return values;
        }
    }
}