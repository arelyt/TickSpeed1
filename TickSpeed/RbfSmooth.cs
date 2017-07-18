using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmooth")]
#pragma warning restore 612
    public class RbfSmoothClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface IRbfSmooth
        {
            double[] rbfsmooth(double[] in1, string in2, double in3, double in4);
        }

        [HandlerParameter(Name = "RBFFunction", NotOptimized = true)]
        public V2.RbfFunction Rbffunc { get; set; }
        [HandlerParameter(Name = "Smooth", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", NotOptimized = false)]
        public double Rbfconst { get; set; }

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
                IRbfSmooth sigDen = client.CreateProxy<IRbfSmooth>(new Uri("http://localhost:9910/rbfsmooth_dep"));
                result = sigDen.rbfsmooth(values, rfunc, Smooth, Rbfconst);
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