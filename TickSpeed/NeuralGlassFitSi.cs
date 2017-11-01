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
    [HandlerName("NeuralFitSi")]
#pragma warning restore 612
    public class NeuralFitSiClass : IDouble2DoubleHandler
    {
        public interface INeuralFitSi
        {
            // ReSharper disable once InconsistentNaming
            double[] myNeuralNetworkFunction(double[] in1);
        }
        //[HandlerParameter(true, "64", Name = "Win")]
        //public int Win { get; set; }

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
                INeuralFitSi sigDen = client.CreateProxy<INeuralFitSi>(new Uri("http://localhost:9910/myNeuralNetworkFunction_dep"));
                values = sigDen.myNeuralNetworkFunction(myDoubles.ToArray());
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