using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using TSLab.DataSource;
using TSLab.Script;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CumDTCWTFlow")]
#pragma warning restore 612
    public class CumDtCwtFlowClass : IBar2DoubleHandler
    {
        public interface ICumDtCwtDen
        {
            // ReSharper disable once InconsistentNaming
            double[] CWTFlow(double[] in1, int in2, int in3);
        }

        [HandlerParameter(true, "31", Name = "LeftBorder", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        public int Lborder { get; set; }

        [HandlerParameter(true, "16", Name = "RightBorder", Max = "159", Min = "1", Step = "1", NotOptimized = false)]
        public int Rborder { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy = trades.Count(trd => trd.Direction == TradeDirection.Buy);
                var valueTickSell = trades.Count(trd => trd.Direction == TradeDirection.Sell);
                var cumtick = valueTickBuy - valueTickSell;
                values[i] = values[i - 1] + cumtick;
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                ICumDtCwtDen sigDen = client.CreateProxy<ICumDtCwtDen>(new Uri("http://localhost:9910/CWTFlow_dep"));
                result = sigDen.CWTFlow(values, Lborder, Rborder);
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