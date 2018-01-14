using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSACompareCycle")]
#pragma warning restore 612
    public class SsaCompareCycleClass : IDouble2DoubleHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface ISsaFor
        {
            double[] ssa2f(double[] in1, double in2, double in3, double in4);
        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numfor { get; set; }

        [HandlerParameter(true, "localhost", Name = "Uri", NotOptimized = true)]
        public string Uria { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {

            var uriafull = "http://" + Uria + ":9910/"; 
            var count = myDoubles.Count;
            if (count < 2)
                return null;

            //var values = new double[count-(int)Numfor];

            var res = new List<double>();
            for (var i = 0; i < (int)Numdec+Numfor-1; i++)
            {
                res.Add(myDoubles[i]);
            }
            // 
            
            // 
            var t = DateTime.Now;
            for (int i = (int)Numdec + (int)Numfor - 1; i < count - Numfor-1; i++)
            {
                var values = new double[(int) Numdec+(int)Numfor-1];
                Array.Copy(myDoubles.ToArray(), i - (int) Numdec + (int)Numfor - 1, values, 0, (int) Numdec-1);
                var result = new double[2 * ((int)Numdec-1)];

                MWClient client = new MWHttpClient();
                try
                {
                    ISsaFor sigDen = client.CreateProxy<ISsaFor>(new Uri(uriafull + "ssa2f_dep"));
                    result = sigDen.ssa2f(values, Numdec, Numrec, Numfor);
                }
                catch (MATLABException)
                {

                }
                finally
                {
                    client.Dispose();
                }
                res.Add(result.Last());
                
                
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssaCompare exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return res;
        }
    }
}