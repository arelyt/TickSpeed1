using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
    [HandlerName("SWTFullCtx")]
    public class SWTFullContext : IOneSourceHandler,
                IDoubleReturns, IStreamHandler, IContextUses,
                IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        // Объемно-тиковый осциллятор - работа с кэшем.
        public IContext Context { set; private get; }

        public enum Wavelets
        {
            Daubechies = 0,
            Symlets = 1

        }

        public enum ThreshRule
        {
            Rigrsure = 0,
            Heursure = 1,
            Sqtwolog = 2,
            Minimaxi = 3,
            Modwtsqtwolog = 4
        }

        public enum Scal
        {
            One = 0,
            Sln = 1,
            Mln = 2
        }

        [HandlerParameter(Name = "Вейвлет", NotOptimized = true)]
        public Wavelets Wave { get; set; }

        [HandlerParameter(true, "1", Name = "Order")]
        public int Order { get; set; }

        [HandlerParameter(true, "3", Name = "Level", Max = "10", Min = "1", Step = "1")]
        public double Level { get; set; }

        [HandlerParameter(Name = "ThresholdRules", NotOptimized = true)]
        public ThreshRule Tptr { get; set; }

        [HandlerParameter(Name = "Scale", NotOptimized = true)]
        public Scal Scale { get; set; }

        public interface ISwtDen
        {
            double[] func_denoise_sw1d_1_auto(double[] in1, string in2, string in3, string in4, double in5);
        }

        public IList<double> Execute(IList<double> myDoubles)
        {
            string name;
            switch (Wave)
            {
                case Wavelets.Daubechies:
                    name = "db";
                    break;
                case Wavelets.Symlets:
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

            var ctx = Context;
            var aCache = ctx.LoadObject("DataCache");
            if (aCache == null)
            {
                var res = SWTFullContext.mps(new_mydoubles, rule, scale, wName, Level);
                ctx.StoreObject("DataCache", res);
                aCache = res;
            }

            var count = myDoubles.Count;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            
            
           

        }

        public static double mps(double[] new_mydoubles, string rl, string sc, string wN, double L)
        {
            // Начинаем Signal denoising process

            // Create client
            MWClient client = new MWHttpClient();
            double[] res;
            try
            {
                ISwtDen sigDen =
                    client.CreateProxy<ISwtDen>(new Uri("http://localhost:9910/func_denoise_sw1d_1_auto_dep"));
                res = sigDen.func_denoise_sw1d_1_auto(new_mydoubles, rl, sc, wN, L);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            return res;
        }
    }
}