using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathNet.Filtering.FIR;
namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("FilterFIRlp")]
#pragma warning restore 612
    public class FilterFirlpClass : IDouble2DoubleHandler
    {
        [HandlerParameter(Name = "CutOff", Default = "0.25", NotOptimized = false)]
        public double Cutoff { get; set; }
        [HandlerParameter(Name = "Order", Default = "32", NotOptimized = false)]
        public int Order { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;
            for (var i = 1; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            IList<double> koeff = FirCoefficients.LowPass(1, Cutoff, Order);
            var f = new OnlineFirFilter(koeff);
            for (var i = 0; i < count; i++)
            {
                values[i] = f.ProcessSample(myDoubles[i]);
            }
            return values;
        }
        
    }

}
