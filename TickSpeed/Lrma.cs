using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MathNet.Filtering.FIR;
using MathNet.Filtering.IIR;

namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("LRMA")]
#pragma warning restore 612
    public class LrmaClass : IDouble2DoubleHandler
    {
       
        [HandlerParameter(Name = "Win", Default = "3", NotOptimized = false)]
        public int Win { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            //var values = new double[count];
            for (var i = 0; i < count; i++)
            {
               result[i] = myDoubles[i];
            }
            //double[] koeff = IirCoefficients.LowPass(1, Cutoff, Order);
            ////var blackmanWindow = new MathNet.Filtering.Windowing.BlackmanWindow {Width = koeff.Count};
            ////var windowArr = blackmanWindow.CopyToArray();
            ////for (int i = 0; i < koeff.Count; i++)
            ////{
            ////    koeff[i] *= windowArr[i];
            ////}
            //var f = new OnlineIirFilter(koeff);
            //var result = f.ProcessSamples(values);
            var t = result.ToList();
            V2.Interpolation.InterpolateNan(ref t);
            result = t.ToArray();
            alglib.filterlrma(ref result, Win);
            
            return result;
        }
        
    }

}
