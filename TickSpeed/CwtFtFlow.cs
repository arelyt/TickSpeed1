using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CWTFTFlow")]
#pragma warning restore 612
    public class CwtFtFlowClass : IDouble2DoubleHandler
    {
        public interface ICwtFtDen
        {
            // ReSharper disable once InconsistentNaming
            double[] CWTFTFlow(double[] in1, int in2, int in3);
        }

        [HandlerParameter(true, "1", Name = "LeftBorder", Max = "15", Min = "1", Step = "1", NotOptimized = false)]
        public int Lborder { get; set; }

        [HandlerParameter(true, "16", Name = "RightBorder", Max = "30", Min = "1", Step = "1", NotOptimized = false)]
        public int Rborder { get; set; }

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
                ICwtFtDen sigDen = client.CreateProxy<ICwtFtDen>(new Uri("http://localhost:9910/CWTFTFlow_dep"));
                result = sigDen.CWTFTFlow(values, Lborder, Rborder);
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