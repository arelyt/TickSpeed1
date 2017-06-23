using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RobustFit")]
#pragma warning restore 612
    public class RobustFitClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public interface IRobustFit
        {
            double[] robustFit(double[] in1, double in2);
        }
        [HandlerParameter(Name = "Window", NotOptimized = false)]
        public double Line { get; set; }

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
            
            // robust fiting last window
            MWClient client = new MWHttpClient();
            try
            {
                IRobustFit sigDen = client.CreateProxy<IRobustFit>(new Uri("http://localhost:9910/robustFit_dep"));
                result = sigDen.robustFit(values, Line);
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