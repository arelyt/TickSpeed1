//using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
//using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Sgolay")]
#pragma warning restore 612
    public class SgolayClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface ISgolay
        {
            double[] sgolay_auto(double[] in1, double in2, double in3);
        }
        [HandlerParameter(true, "5", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Order { get; set; }

        [HandlerParameter(true, "33", Name = "Win", Max = "99", Min = "5", Step = "1", NotOptimized = false)]
        public int Win { get; set; }

        [HandlerParameter(true, "0", Name = "Deriv", Max = "3", Min = "0", Step = "1", NotOptimized = false)]
        public int Deriv { get; set; }

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
            // Начинаем Savitzky-Golay process

            SavitzkyGolay sg = new SavitzkyGolay(Win, Deriv, Order);
            sg.Apply(values, result);
            //MWClient client = new MWHttpClient();
            //try
            //{
            //    ISgolay sigDen = client.CreateProxy<ISgolay>(new Uri("http://localhost:9910/sgolay_auto_dep"));
            //    result = sigDen.sgolay_auto(values, Order, Win);
            //}
            //catch (MATLABException)
            //{

            //}
            //finally
            //{
            //    client.Dispose();
            //}
            return result;
        }
    }
}