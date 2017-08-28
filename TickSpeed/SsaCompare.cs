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
            double[] Ssa_For(double[] in1, int in2, int in3, int in4);
        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public int Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numrec { get; set; }

        [HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numfor { get; set; }

        [HandlerParameter(true, "localhost", Name = "Uri", NotOptimized = true)]
        public string Uria { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {

            var uriafull = "http://" + Uria + ":9910/"; 
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            var values = new double[count-Numfor];
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
                ISsaFor sigDen = client.CreateProxy<ISsaFor>(new Uri(uriafull + "Ssa_For_dep"));
                result = sigDen.Ssa_For(values, Numdec, Numrec, Numfor);
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