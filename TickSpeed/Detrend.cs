using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Detrend")]
#pragma warning restore 612
    public class DetrendClass : IDouble2DoubleHandler
    {
        public interface IDetrend
        {
            // ReSharper disable once InconsistentNaming
            double[] detrend1(double[] in1, double in2);
        }
        [HandlerParameter(Name = "Step", NotOptimized = false)]
        public double Step { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            //var l = (int)Math.Floor(count / Step);
            //var vector = new double[l];
            //vector[0] = 0;
            //for (var i = 1; i < l; i++)
            //{
            //    vector[i] = vector[i-1] + Step;
            //}


            MWClient client = new MWHttpClient();
            try
            {
                IDetrend sigDen = client.CreateProxy<IDetrend>(new Uri("http://localhost:9910/detrend1_dep"));
                values = sigDen.detrend1(myDoubles.ToArray(), Step);
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
