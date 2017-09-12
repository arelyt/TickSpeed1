using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using RusAlgo.Helper;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("IBY")]
#pragma warning restore 612
    public class IbySsaSgClass : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface IIby
        {
            // ReSharper disable once InconsistentNaming
            double[] IBY1(double[] in1, double in2, double in3, double in4, double in5, double in6, double in7);
        }

        [HandlerParameter(true, "60", Name = "Numdec", Max = "1000", Min = "10", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "5", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Order { get; set; }

        [HandlerParameter(true, "61", Name = "winSG", Max = "120", Min = "1", Step = "1", NotOptimized = false)]
        public double WinSg { get; set; }

        [HandlerParameter(true, "0", Name = "deriv", Max = "3", Min = "0", Step = "1", NotOptimized = true)]
        public double Deriv { get; set; }

        [HandlerParameter(true, "1", Name = "sampleT", Max = "10", Min = "0", Step = "1", NotOptimized = true)]
        public double Dtime { get; set; }

        //[HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numfor { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            if (myDoubles.IsNull())
                return myDoubles;
            var count = myDoubles.Count;
            if (count < Numdec + 2)
                return myDoubles;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            var t = DateTime.Now;
            MWClient client = new MWHttpClient();
            try
            {
                IIby sigDen = client.CreateProxy<IIby>(new Uri("http://localhost:9910/IBY1_dep"));
                result = sigDen.IBY1(values, Numdec, Numrec, Order, WinSg, Deriv, Dtime);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssa exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}