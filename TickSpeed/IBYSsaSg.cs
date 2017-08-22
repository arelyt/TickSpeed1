using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("IBY")]
#pragma warning restore 612
    public class IbySsaSgClass : IDouble2DoubleHandler
    {
        public interface IIby
        {
            // ReSharper disable once InconsistentNaming
            double[] IBY(double[] in1, int in2, int in3, int in4, int in5, int in6, double in7);
        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public int Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numrec { get; set; }

        [HandlerParameter(true, "5", Name = "order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Order { get; set; }

        [HandlerParameter(true, "61", Name = "window", Max = "120", Min = "1", Step = "1", NotOptimized = false)]
        public int Win { get; set; }

        [HandlerParameter(true, "0", Name = "deriv", Max = "3", Min = "0", Step = "1", NotOptimized = false)]
        public int Deriv { get; set; }

        [HandlerParameter(true, "1", Name = "sampleT", Max = "10", Min = "0", Step = "1", NotOptimized = false)]
        public int Dtime { get; set; }

        //[HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numfor { get; set; }

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
                IIby sigDen = client.CreateProxy<IIby>(new Uri("http://localhost:9910/IBY_dep"));
                result = sigDen.IBY(values, Numdec, Numrec, Order, Win, Deriv, Dtime);
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