using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("SimpleSWT")]
    public class SimpleSWTClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_auto(double[] in1);
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
                ISwtDen sigDen = client.CreateProxy<ISwtDen>(new Uri("http://localhost:9910/func_denoise_sw1d_1_auto_dep"));
                result = sigDen.func_denoise_sw1d_1_auto(values);
            }
            catch (MATLABException ex)
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