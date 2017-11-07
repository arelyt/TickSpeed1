using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // недоделано
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("JAFilter")]
#pragma warning restore 612
    public class JaFilterClass : IDouble2DoubleHandler
    {
        [HandlerParameter(Name = "Lambda", Default = "0.25", NotOptimized = false)]
        public double Lambda { get; set; }
        //[HandlerParameter(Name = "Order", Default = "32", NotOptimized = false)]
        //public int Order { get; set; }
        public interface IJaFilter
        {
            // ReSharper disable once InconsistentNaming
            double[] jafilter(double[] in1, double in2);
        }
        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            
            //for (var i = 0; i < count; i++)
            //{
            //    values[i] = myDoubles[i];
            //}
            MWClient client = new MWHttpClient();
            try
            {
                IJaFilter sigDen = client.CreateProxy<IJaFilter>(new Uri("http://localhost:9910/jafilter_dep"));
                values = sigDen.jafilter(myDoubles.ToArray(), Lambda);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }

            return values;
        }
        
    }

}
