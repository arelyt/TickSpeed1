using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CasheFunTest")]
#pragma warning restore 612
    public class CasheFunTestClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface ICasheFun
        {
            double[] cashefun(double[] in1);
        }
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }

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
                result = sigDen.cashefun(values);
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