using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.Script;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmooth")]
#pragma warning restore 612
    public class RbfSmoothSecTimeClass : IBar2DoubleHandler
    {
        public interface IRbfSmoothSecTime
        {
            double[] rbfsmoothsectime(double[] in1, double[] in2, string in3, double in4, double in5);
        }

        [HandlerParameter(Name = "RBFFunction", NotOptimized = true)]
        public V2.RbfFunction Rbffunc { get; set; }
        [HandlerParameter(Name = "Smooth", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", NotOptimized = false)]
        public double Rbfconst { get; set; }

        public IList<double> Execute(ISecurity security)
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
                time[i] = security.Bars[i].Date.TimeOfDay.TotalMilliseconds;
            }
            // Начинаем Signal denoising process
            string rfunc;
            switch (Rbffunc)
            {
                case V2.RbfFunction.Gaussian:
                    rfunc = "gaussian";
                    break;
                case V2.RbfFunction.Thinplate:
                    rfunc = "thinplate";
                    break;
                case V2.RbfFunction.Cubic:
                    rfunc = "cubic";
                    break;
                case V2.RbfFunction.Multicuadrics:
                    rfunc = "multicuadrics";
                    break;
                case V2.RbfFunction.Linear:
                    rfunc = "l8near";
                    break;
                default:
                    rfunc = "multiquadrics";
                    break;

            }
            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                IRbfSmoothSecTime sigDen = client.CreateProxy<IRbfSmoothSecTime>(new Uri("http://localhost:9910/rbfsmoothsectime_dep"));
                result = sigDen.rbfsmoothsectime(time, values, rfunc, Smooth, Rbfconst);
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