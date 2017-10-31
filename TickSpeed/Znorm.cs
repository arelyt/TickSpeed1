using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Скользящее нормализованное окно накопительной дельты
#pragma warning disable 612
    [HandlerCategory("Arelyt")]
    [HandlerName("ZNorm")]
#pragma warning restore 612
    public class ZNormClass : IDouble2DoubleHandler
    {
        public interface IZnorm
        {
            double[] znorm(double[] in1, double in2);
        }
        [HandlerParameter(true, "64", Name = "Win")]
        public int Win { get; set; }

        //[HandlerParameter(true, "0.5", Name = "K")]
        //public double K { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count]; // values result
           
            MWClient client = new MWHttpClient();
            try
            {
                IZnorm sigDen = client.CreateProxy<IZnorm>(new Uri("http://localhost:9910/znorm_dep"));
                values = sigDen.znorm(myDoubles.ToArray(), Win);
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