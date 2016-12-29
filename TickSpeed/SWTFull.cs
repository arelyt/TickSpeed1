using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
   
    public enum Wavelets
    {
        Dobeshy = 0,
        Morle = 1

    }
    [HandlerCategory("Arelyt")]
    [HandlerName("SWTFull")]
    public class SWTFullClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        [HandlerParameter(Name = "Вейвлет", NotOptimized = true)]
        public Wavelets Wave { get; set; }

        [HandlerParameter(true, "1", Name = "WaveletBase(1-DB, 2-SYM)", Max = "2", Min = "1", Step = "1")]
        public int WaveletBase { get; set; }

        [HandlerParameter(true, "3", Name = "Level", Max = "7", Min = "1", Step = "1")]
        public double Level { get; set; }

       public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_auto(double[] in1, string in2, double in3);
        }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var Name = "";
            switch (WaveletBase)
            {
                case 1:
                    Name = "db3";
                    break;
                case 2:
                    Name = "sym3";
                    break;
                default:
                    Name = "db3";
                    break;
            }
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
                result = sigDen.func_denoise_sw1d_1_auto(values, Name, Level);
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