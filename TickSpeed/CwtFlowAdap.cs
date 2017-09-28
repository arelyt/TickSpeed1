using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("CWTFlowAdaptive")]
    public class CwtFlowAdaptiveClass : IDouble2DoubleHandler
    {
        public interface ICwtDen
        {
            double[] CWTFlow1(double[] in1, int in2);
        }

        //[HandlerParameter(true, "65", Name = "LeftBorder", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        //public int Lborder { get; set; }

        [HandlerParameter(true, "0", Name = "Offset", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        public int Offset { get; set; }

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

            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                ICwtDen sigDen = client.CreateProxy<ICwtDen>(new Uri("http://localhost:9910/CWTFlow1_dep"));
                result = sigDen.CWTFlow1(values, Offset);
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