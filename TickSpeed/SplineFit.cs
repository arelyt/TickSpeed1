using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SpineFit")]
#pragma warning restore 612
    public class SplineFitClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface ISplineFit
        {
            double[] splinefit_auto(double[] in1, double in2, double in3);
        }
        [HandlerParameter(Name = "Breaks", NotOptimized = false)]
        public double Breaks { get; set; }
        [HandlerParameter(Name = "Order", NotOptimized = false)]
        public double Order { get; set; }
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
            // Начинаем Spline fitting process
            
            
            MWClient client = new MWHttpClient();
            try
            {
                ISplineFit sigDen = client.CreateProxy<ISplineFit>(new Uri("http://localhost:9910/splinefit_auto_dep"));
                result = sigDen.splinefit_auto(values, Breaks, Order);
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