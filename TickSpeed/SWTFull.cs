using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
   
    public enum Wavelets
    {
        Daubechies = 0,
        Symlets = 1

    }
    [HandlerCategory("Arelyt")]
    [HandlerName("SWTFull")]
    public class SWTFullClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        [HandlerParameter(Name = "Вейвлет", NotOptimized = true)]
        public Wavelets Wave { get; set; }

        [HandlerParameter(true, "1", Name = "Order")]
        public int order { get; set; }

        [HandlerParameter(true, "3", Name = "Level", Max = "10", Min = "1", Step = "1")]
        public double Level { get; set; }

       public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_auto(double[] in1, string in2, double in3);
        }

        public IList<double> Execute(IList<double> myDoubles)
        {
            string name;
            switch (Wave)
            {
                case Wavelets.Daubechies:
                    name = "db";
                    break;
                case Wavelets.Symlets:
                    name = "sym";
                    break;
                
                default:
                    name = "db";
                    break;
            }
            var wName = name + order.ToString();

            var count = myDoubles.Count;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            // Create client
            MWClient client = new MWHttpClient();
            try
            {
                ISwtDen sigDen = client.CreateProxy<ISwtDen>(new Uri("http://localhost:9910/func_denoise_sw1d_1_auto_dep"));
                result = sigDen.func_denoise_sw1d_1_auto(values, wName, Level);
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