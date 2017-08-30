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
    [HandlerName("CasheFunTest")]
#pragma warning restore 612
    public class CasheFunTestClass : IBar2DoubleHandler
    {
        
        public interface ICasheFun
        {
            // ReSharper disable once InconsistentNaming
            double[] cashefun(double[] in1, double[] in2, double[] in3);
        }
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            var count = sec.Bars.Count;
            if (count < 100)
                return null;
            var result = new double[count];
            var price = new double[count];
            var tradeno = new double[count];
            //var askq = new double[count];
            //var bidq = new double[count];
            //var askp = new double[count];
            //var bidp = new double[count];
            var time = new double[count];
            for (var i = 0; i < count; i++)
            {
                price[i] = sec.Bars[i].Close;
                
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                time[i] = sec.Bars[i].Date.TimeOfDay.TotalMilliseconds - 36000;
            }
            // Начинаем Signal denoising process
            //string line;
            //switch (Line)
            //{
            //    case V2.Predin.Upper:
            //        line = "upper";
            //        break;

            //    case V2.Predin.Lower:
            //        line = "lower";
            //        break;
            //    case V2.Predin.Values:
            //        line = "values";
            //        break;
            //    default:
            //        line = "values";
            //        break;

            //}
            // Wavelet DB3 Level 5
            MWClient client = new MWHttpClient();
            try
            {
                ICasheFun sigDen = client.CreateProxy<ICasheFun>(new Uri("http://localhost:9910/cashefun_dep"));
                result = sigDen.cashefun(time, tradeno, price);
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