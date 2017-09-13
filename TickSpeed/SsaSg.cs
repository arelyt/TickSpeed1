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
    [HandlerName("SSAsg")]
#pragma warning restore 612
    public class SsaSgClass : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface ISsaFor
        {
            
            double[] ssa1(double[] in1, double in2, double in3);

        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public double Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public double Numrec { get; set; }

        [HandlerParameter(true, "5", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Order { get; set; }

        [HandlerParameter(true, "61", Name = "winSG", Max = "120", Min = "1", Step = "1", NotOptimized = false)]
        public int WinSg { get; set; }

        [HandlerParameter(true, "0", Name = "deriv", Max = "3", Min = "0", Step = "1", NotOptimized = true)]
        public int Deriv { get; set; }

        //[HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numfor { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            if (myDoubles.IsNull())
                return myDoubles;
            var count = myDoubles.Count;
            if (count < Numdec + 2)
                return myDoubles;
            var t = DateTime.Now;
            var result = new double[count];
            var values = new double[count];
            var result1 = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            
            MWClient client = new MWHttpClient();
            try
            {
                ISsaFor sigDen = client.CreateProxy<ISsaFor>(new Uri("http://localhost:9910/ssa1_dep"));
                result1 = sigDen.ssa1(values, Numdec, Numrec);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            SavitzkyGolay sg = new SavitzkyGolay(WinSg, Deriv, Order);
            sg.Apply(result1, result);
            Context.Log("ssasg exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }

        

        
    }
}