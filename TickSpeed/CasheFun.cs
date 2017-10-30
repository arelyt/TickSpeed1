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
    [HandlerName("CasheFun")]
#pragma warning restore 612
    public class CasheFunClass : IThreeSourcesHandler, ISecurityInput0,
                                 IDoubleInput1, IDoubleInput2, IDoubleReturns,
                                 IContextUses
    {
       
        
        public interface ICasheFun1
        {
            // ReSharper disable once InconsistentNaming
            double[] cashefun1(double[] in1, double[] in2, double[] in3);
        }
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }

        public IList<double> Execute(ISecurity sec, IList<double> halfspread, IList<double> glassosc)
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
                price[i] = halfspread[i];
                
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                time[i] = sec.Bars[i].Date.TimeOfDay.TotalMilliseconds - 36000;
            }
            
            MWClient client = new MWHttpClient();
            try
            {
                ICasheFun1 sigDen = client.CreateProxy<ICasheFun1>(new Uri("http://localhost:9910/cashefun1_dep"));
                result = sigDen.cashefun1(time, tradeno, price);
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


        public IContext Context { get; set; }
    }
}