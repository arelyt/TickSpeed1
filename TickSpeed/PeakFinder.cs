using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Поиск пиков
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("PeakFinder")]
#pragma warning restore 612
    public class PeakFinderClass : IDoubleCompaper1Handler
    {
        public interface IPeakFinder
        {
            Boolean[] peakfinder(double[] in1, double in2, double in3, double in4, bool in5, bool in6);
        }

        [HandlerParameter(Name = "Select", NotOptimized = true)]
        public double Sel { get; set; }

        [HandlerParameter(Name = "Thresh", Default = "100")]
        public double Thresh { get; set; }

        [HandlerParameter(Name = "Extrema", Default = "1", NotOptimized = true)]
        public double Extrem { get; set; }

        [HandlerParameter(Name = "EndPoints", Default = "True", NotOptimized = true)]
        public bool Endpoints { get; set; }

        [HandlerParameter(Name = "Interp", Default = "True", NotOptimized = true)]
        public bool Interp { get; set; }

        public IList<Boolean> Execute(IList<double> myDoubles)
        {
            
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var result = new Boolean[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Envelope process

            
            MWClient client = new MWHttpClient();
            try
            {
                IPeakFinder sigDen = client.CreateProxy<IPeakFinder>(new Uri("http://localhost:9910/peakfinder_dep"));
                result = sigDen.peakfinder(values, Sel, Thresh, Extrem, Endpoints, Interp);
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