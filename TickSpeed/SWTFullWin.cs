using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TickSpeed.V2;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
   


    [HandlerCategory("Arelyt")]
    [HandlerName("SWTFullWin")]
    public class SwtFullWinClass : IDouble2DoubleHandler
    {
        [HandlerParameter(Name = "Вейвлет", NotOptimized = true)]
        public V2.Wavelets Wave { get; set; }

        [HandlerParameter(true, "1", Name = "Order")]
        public int Order { get; set; }

        [HandlerParameter(true, "3", Name = "Level", Max = "10", Min = "1", Step = "1")]
        public double Level { get; set; }

        [HandlerParameter(Name = "ThresholdRules", NotOptimized = true)]
        public ThreshRule Tptr { get; set; }

        [HandlerParameter(Name = "Scale", NotOptimized = true)]
        public Scal Scale { get; set; }

        [HandlerParameter(Name = "Window", NotOptimized = true)]
        public int Win { get; set; }

        public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_auto(double[] in1, string in2, string in3, string in4, double in5);
        }

        public IList<double> Execute(IList<double> myDoubles)
        {
            string name;
            switch (Wave)
            {
                case V2.Wavelets.Daubechies:
                    name = "db";
                    break;
                case V2.Wavelets.Symlets:
                    name = "sym";
                    break;
                
                default:
                    name = "db";
                    break;
            }
            string rule;
            switch (Tptr)
            {
                 case ThreshRule.Rigrsure:
                    rule = "rigrsure";
                    break;
                 case ThreshRule.Heursure:
                    rule = "heursure";
                    break;
                 case ThreshRule.Sqtwolog:
                    rule = "sqtwolog";
                    break;
                 case ThreshRule.Minimaxi:
                    rule = "minimaxi";
                    break;
                 case ThreshRule.Modwtsqtwolog:
                    rule = "modwtsqtwolog";
                    break;
                default:
                    rule = "modwtsqtwolog";
                    break;
            }
            string scale;
            switch (Scale)
            {
                 case Scal.One:
                    scale = "one";
                    break;
                 case Scal.Sln:
                    scale = "sln";
                    break;
                 case Scal.Mln:
                    scale = "mln";
                    break;
                default:
                    scale = "mln";
                    break;
            }
            var wName = name + Order.ToString();

            var count = myDoubles.Count;
            var result = new double[count];
            var res = new double[count];
            var values = myDoubles.Skip(count - Win).Take(Win).ToArray();

            for (var i = 0; i < count; i++)
            {
                res[i] = 0.0;
            }
            // Начинаем Signal denoising process

            // Create client
            MWClient client = new MWHttpClient();
            try
            {
                ISwtDen sigDen = client.CreateProxy<ISwtDen>(new Uri("http://localhost:9910/func_denoise_sw1d_1_auto_dep"));
                result = sigDen.func_denoise_sw1d_1_auto(values, rule, scale, wName, Level);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            for (int i = count-Win+1; i < count; i++)
            {
                res[i] = result[i-count+Win-1];
            }
            return res;
        }

    }
}