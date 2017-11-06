using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathNet.Filtering.FIR;
using MathNet.Filtering.IIR;

namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("FilterIIRlp")]
#pragma warning restore 612
    public class FilterIirlpClass : IDouble2DoubleHandler
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
            
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            double[] koeff = IirCoefficients.LowPass(1, Cutoff, Order);
            //var blackmanWindow = new MathNet.Filtering.Windowing.BlackmanWindow {Width = koeff.Count};
            //var windowArr = blackmanWindow.CopyToArray();
            //for (int i = 0; i < koeff.Count; i++)
            //{
            //    koeff[i] *= windowArr[i];
            //}
            var f = new OnlineIirFilter(koeff);
            var result = f.ProcessSamples(values);
            
            return result;
        }
        
    }

}
