using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("WSSTFlow")]
    public class WsstFlowClass : IDouble2DoubleHandler
    {
        public interface ICwtDen
        {
            double[] WSSTFlow(double[] in1, double in2, double in3, double in4);
        }

        [HandlerParameter(true, "0.01", Name = "LowFreq", Max = "0.5", Min = "0.0001", Step = "0.0001", NotOptimized = false)]
        public double LFreq { get; set; }

        [HandlerParameter(true, "0.03", Name = "HighFreq", Max = "0.5", Min = "0.0001", Step = "0.0001", NotOptimized = false)]
        public double HFreq { get; set; }

        [HandlerParameter(true, "1", Name = "SampleFreq", Max = "100", Min = "1", Step = "1", NotOptimized = false)]
        public double SamplingFreq { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            MWClient client = new MWHttpClient();
            try
            {
                ICwtDen sigDen = client.CreateProxy<ICwtDen>(new Uri("http://localhost:9910/WSSTFlow_dep"));
                result = sigDen.WSSTFlow(values, LFreq, HFreq, SamplingFreq);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            return result;
        }
    }
}