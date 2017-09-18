using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using RusAlgo.Helper;

// ReSharper disable InconsistentNaming

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("PE")]
#pragma warning restore 612
    public class PeClass : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface IPe
        {
            
            double[] PE(double[] in1, double in2, double in3, double in4);

        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Winsize { get; set; }

        [HandlerParameter(true, "3", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Order { get; set; }

        [HandlerParameter(true, "1", Name = "Delay", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Delay { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            if (myDoubles.IsNull())
                return myDoubles;
            var count = myDoubles.Count;
            if (count < Winsize + 2)
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
                IPe sigDen = client.CreateProxy<IPe>(new Uri("http://localhost:9910/PE_dep"));
                result = sigDen.PE(values, Delay, Order, Winsize);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("PE exec for "+g+" msec", MessageType.Info, toMessageWindow:true);
            return result;
        }

        

        
    }
}