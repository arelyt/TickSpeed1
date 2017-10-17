using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmoothNetrbfit")]
#pragma warning restore 612
    public class RbfSmoothNetrbfitClass : IOneSourceHandler, ISecurityInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        //[HandlerParameter(Name = "NLayer", Default = "3", NotOptimized = false)]
        //public int Nlayer { get; set; }
        [HandlerParameter(Name = "Spread", Default = "0.01", NotOptimized = false)]
        public double Spread { get; set; }
        [HandlerParameter(Name = "Goal", Default = "2.0", NotOptimized = false)]
        public double Goal { get; set; }

        //private rbfmodel _model;
        public IContext Context { get; set; }
        public interface IRbfSmoothNetrbfit
        {
            // ReSharper disable once InconsistentNaming
            double[] netrbfit(double[] in1, double[] in2, double in3, double in4);
        }

        public IList<double> Execute(ISecurity security)
        {
            var t = DateTime.Now;
            var count = security.Bars.Count;
            if (count < 10)
                return null;
            
            var result = new double[count];
            var values = new double[count];
            var time = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = security.Bars[i].Close;
                time[i] = i;
            }
            
            MWClient client = new MWHttpClient();
            try
            {
                var sigDen = client.CreateProxy<IRbfSmoothNetrbfit>(new Uri("http://localhost:9910/netrbfit_dep"));
                result = sigDen.netrbfit(time, values, Goal, Spread);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("netrbfit exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result.Take(count).ToList();
        }
    }
}