using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MoreLinq;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumDTCWTFlow1")]
#pragma warning restore 612
    public class CumDtCwtFlow1Class : ITwoSourcesHandler, IDoubleInputs, IDoubleReturns, IValuesHandlerWithNumber, IContextUses
    {
        private double[] _data;
        public static IList<double> Cacheflow { get; set; }
        public interface ICumDtCwtDen
        {
            // ReSharper disable once InconsistentNaming
            double[] CWTFlow1(double[] in1, int in2);
        }

        //[HandlerParameter(true, "31", Name = "LeftBorder", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        //public int Lborder { get; set; }
        [HandlerParameter(true, "16", Name = "RightBorder", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        public int Rborder { get; set; }
        [HandlerParameter(true, "1024", Name = "Win", Max = "4096", Min = "256", Step = "1", NotOptimized = false)]
        public int Win { get; set; }
        public IContext Context { set => throw new NotImplementedException(); }

        public double Execute(double ctd, int barNum, IContext ctx)
        {
            var count = ctx.BarsCount;
            if (_data == null)
                _data = new double[ctx.BarsCount];

            _data[barNum] = ctd;
            if (count < Win)
                return 0;
            var dt = _data.TakeLast(Win).ToArray();

            return Execute(dt);
        }

        protected double Execute(double[] data)
        {
            var values = 0.0;
            MWClient client = new MWHttpClient();
            try
            {
                ICumDtCwtDen sigDen =
                    client.CreateProxy<ICumDtCwtDen>(new Uri("http://localhost:9910/CWTFlow1_dep"));
                values = sigDen.CWTFlow1(data, Rborder).Last();
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