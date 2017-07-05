using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TickSpeed.V2;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CWTFTFlow")]
#pragma warning restore 612
    public class CwtFtFlowClass : IDouble2DoubleHandler
    {
        public interface ICwtFtDen
        {
            // ReSharper disable once InconsistentNaming
            double[] CWTFTFlow(double[] in1, int in2, int in3, string in4);
        }

        [HandlerParameter(true, "1", Name = "LeftBorder", Max = "15", Min = "1", Step = "1", NotOptimized = false)]
        public int Lborder { get; set; }

        [HandlerParameter(true, "16", Name = "RightBorder", Max = "30", Min = "1", Step = "1", NotOptimized = false)]
        public int Rborder { get; set; }

        [HandlerParameter(Name = "Extmode", NotOptimized = true)]
        public Extmode Mode { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            string mode;
            switch (Mode)
            {
                case Extmode.Zpd:
                    mode = "zpd";
                    break;
                case Extmode.Sp0:
                    mode = "sp0";
                    break;
                case Extmode.Sp1:
                    mode = "sp1";
                    break;
                
                case Extmode.Sym:
                    mode = "sym";
                    break;
                case Extmode.Symw:
                    mode = "symw";
                    break;
                case Extmode.Asym:
                    mode = "asym";
                    break;
                case Extmode.Asymw:
                    mode = "asymw";
                    break;
                case Extmode.Ppd:
                    mode = "ppd";
                    break;
                case Extmode.Per:
                    mode = "per";
                    break;
                default:
                    mode = "sp1";
                    break;
            }
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

            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                ICwtFtDen sigDen = client.CreateProxy<ICwtFtDen>(new Uri("http://localhost:9910/CWTFTFlow_dep"));
                result = sigDen.CWTFTFlow(values, Lborder, Rborder, mode);
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