using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Fit")]
#pragma warning restore 612
    public class FitClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface IFit
        {
            double[] Fit1(double[] in1);
        }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
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
                IFit sigDen = client.CreateProxy<IFit>(new Uri("http://localhost:9910/Fit1_dep"));
                result = sigDen.Fit1(values);
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