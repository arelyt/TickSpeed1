using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
    [HandlerName("SWTDEN6")]
    public class Swtden6Class : IBar2DoubleHandler
    {
        public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_6_auto(double[] in1);
        }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            var values = new double[count];
            var result = new double[count];

            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy = 0.0;
                var valueTickSell = 0.0;
                var valueVolBuy = 0.0;
                var valueVolSell = 0.0;

                foreach (var t in trades)
                {
                    var trd = t;
                    valueTickBuy += t.Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                    valueTickSell += t.Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                }
                // Считаем осциллятор
            
                    values[i] = ((valueTickBuy*valueVolBuy - valueTickSell*valueVolSell)/
                                 (valueTickBuy*valueVolBuy + valueTickSell*valueVolSell));
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                ISwtDen sigDen = client.CreateProxy<ISwtDen>(new Uri("http://localhost:9910/func_denoise_sw1d_1_6_auto_dep"));
                result = sigDen.func_denoise_sw1d_1_6_auto(values);
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