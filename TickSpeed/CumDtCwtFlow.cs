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
        public static IList<double> Cacheflow { get; set; }

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
            if (count < 5)
                return null;
            //var result = new double[count];
            
            if (Cacheflow == null || Cacheflow.Count < count - 1)
            {

                var result = Tratata(security, Lborder, Rborder);
                Cacheflow = result;
                return Cacheflow;

            }
            else

            {
                //var cdiff = count - Cacheflow.Count;
                var t = new double[count];
                
                var result = Tratata(security, Lborder, Rborder);
                
                for (int i = 0; i < count - 1; i++)
                {
                    t[i] = Cacheflow[i + 1];
                }
                //for (int i = count-cdiff; i < count; i++)
                //{
                //    t[i] = result[i];
                //}
                //for (int i = 0; i < UPPER; i++)
                //{
                    
                //}
                t[count] = result[count];
                Cacheflow = t;
                return Cacheflow;
            }
            
        }

        public static double[] Tratata(ISecurity security, int lborder, int rborder)
        {
            var values = new double[security.Bars.Count];
            values[0] = 0.0;
            for (var i = 1; i < security.Bars.Count; i++)
            {
                var trades = security.GetTrades(i);
                var valueTickBuy = trades.Count(trd => trd.Direction == TradeDirection.Buy);
                var valueTickSell = trades.Count(trd => trd.Direction == TradeDirection.Sell);
                var cumtick = valueTickBuy - valueTickSell;
                values[i] = values[i - 1] + cumtick;
            }
            // Начинаем CWT_ICWT denoising process


            MWClient client = new MWHttpClient();
            try
            {
                ICumDtCwtDen sigDen =
                    client.CreateProxy<ICumDtCwtDen>(new Uri("http://localhost:9910/CWTFlow_dep"));
                values = sigDen.CWTFlow(values, lborder, rborder);
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