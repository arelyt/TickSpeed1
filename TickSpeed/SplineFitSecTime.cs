using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script;

namespace TickSpeed
{
    //SplineFitting
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SpineFitSecTime")]
#pragma warning restore 612
    public class SplineFitSecTimeClass : IBar2DoubleHandler
    {
        public interface ISplineFitSecTime
        {
            double[] splinefitsectime_auto(double[] in1, double[] in2, double in3, double in4);
        }
        [HandlerParameter(Name = "Breaks", NotOptimized = false)]
        public double Breaks { get; set; }
        [HandlerParameter(Name = "Order", NotOptimized = false)]
        public double Order { get; set; }
        public IList<double> Execute(ISecurity security )
        {
            var count = security.Bars.Count;
            if (count < 10)
                return null;
            var result = new double[count];
            var values = new double[count];
            var time = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = security.Bars[i].Close;
                time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds - security.Bars[0].Date.TimeOfDay.TotalSeconds;
            }
            // Начинаем Spline fitting process
            
            
            MWClient client = new MWHttpClient();
            try
            {
                ISplineFitSecTime sigDen = client.CreateProxy<ISplineFitSecTime>(new Uri("http://localhost:9910/splinefitsectime_auto_dep"));
                result = sigDen.splinefitsectime_auto(time, values, Breaks, Order);
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