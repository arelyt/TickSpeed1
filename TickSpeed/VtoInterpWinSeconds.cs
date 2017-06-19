using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script;

namespace TickSpeed
{
    // Полный осциллятор потока + произвольный
    // тиковый таймфрейм + вейвлет-шумоподавление +
    // интерполяция сигнала по тиковому таймфрейму
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("VTOInSec")]
#pragma warning restore 612
    public class VtoInterpWinSecondsClass : IBar2DoubleHandler
    {
        public interface IVtoInterpolSeconds
        {
            double[] vtointerpsec_auto(double[] in1, double[] in2, double in3);
        }
        //[HandlerParameter(Name = "Order", NotOptimized = false)]
        //public double Order { get; set; }
        [HandlerParameter(Name = "Win", NotOptimized = false)]
        public double Win { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            if (security.IntervalBase.ToString() != "TICK" || security.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            var values1 = new double[count];
            var values2 = new double[count];
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                foreach (var t in trades)
                {
                    var trd = t;
                    values1[i] += t.Direction.ToString() == "Buy" ? trd.Quantity : 0;
                    values2[i] += t.Direction.ToString() == "Sell" ? trd.Quantity : 0;
                }
            }
            // Начинаем wden + interpolation process


            MWClient client = new MWHttpClient();
            try
            {
                IVtoInterpolSeconds sigDen = client.CreateProxy<IVtoInterpolSeconds>(new Uri("http://localhost:9910/vtointerpsec_auto_dep"));
                result = sigDen.vtointerpsec_auto(values1, values2, Win);
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