using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSACompare")]
#pragma warning restore 612
    public class SsaCompareClass : IDouble2DoubleHandler, IContextUses
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
            var result = new double[count];
            var values = new double[count-(int)Numfor];
            for (var i = 0; i < count-Numfor; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            var t = DateTime.Now;
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
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssaCompare exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}