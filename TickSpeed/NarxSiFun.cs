﻿using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
using MoreLinq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // NARX MATLAB
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("NARXSiFun")]
#pragma warning restore 612
    public class NarxSiFunClass : IStreamHandler, IThreeSourcesHandler, ISecurityInput0,
                                 IDoubleInput1, IDoubleInput2, IDoubleReturns, IContextUses
    {
        public interface INarxSiFun
        {
            // ReSharper disable once InconsistentNaming
            double[] narxsifun(double[] in1, double[] in2, double[] in3, double[] in4, double[] in5);
        }
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }
        [HandlerParameter(Name = "Win", NotOptimized = false)]
        public int Win { get; set; }
        [HandlerParameter(true, "5", Name = "WinExp", NotOptimized = false)]
        public int WinExp { get; set; }
        [HandlerParameter(true, "600", Name = "WinMain", NotOptimized = false)]
        public int WinMain { get; set; }
        [HandlerParameter(true, "0.01", Name = "K", NotOptimized = false)]
        public double K { get; set; }
        public IContext Context { get; set; }

        public IList<double> Execute(ISecurity sec, IList<double> halfspread, IList<double> glassosc)
        {
            var ctx = Context;
            var count = sec.Bars.Count;
            if (count < 100)
                return null;
            var values = new double[WinMain]; // values result
            var result = new double[count];
            var price = new double[count];
            var tradeno = new double[count];
            IList<double> nBuy = new double[count];
            IList<double> nSell = new double[count];
            //var askq = new double[count];
            //var bidq = new double[count];
            //var askp = new double[count];
            //var bidp = new double[count];
            var time = new double[count];
            var ask = sec.GetSellQueue(0)[0].Price;
            var bid = sec.GetBuyQueue(0)[0].Price;
            for (var i = 0; i < count; i++)
            {
                price[i] = halfspread[i];
                
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                time[i] = sec.Bars[i].Date.TimeOfDay.TotalMilliseconds - 36000;
            }

            for (var i = Win - 1; i < count; i++)
            {
                var tradelastwin = sec.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                nBuy[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Buy ? 1 : 0) / (double)Win;
                nSell[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Sell ? 1 : 0) / (double)Win;
            }
            nBuy = Series.EMA(nBuy, WinExp);
            nSell = Series.EMA(nSell, WinExp);
            var osc = new double[count];
            for (int i = 0; i < count; i++)
            {
                if (Math.Abs(nSell[i]) > 0.0001)
                {
                    osc[i] = Math.Tanh(K * Math.Log(nBuy[i] + nSell[i]) * (nBuy[i] - nSell[i]));
                }
                else
                {
                    osc[i] = 0.0;
                }
            }
            MWClient client = new MWHttpClient();
            try
            {
                INarxSiFun sigDen = client.CreateProxy<INarxSiFun>(new Uri("http://localhost:9910/narxsifun_dep"));
                result = sigDen.narxsifun(time.TakeLast(WinMain).ToArray(), tradeno.TakeLast(WinMain).ToArray(),
                            price.TakeLast(WinMain).ToArray(), osc.TakeLast(WinMain).ToArray(),
                            glassosc.TakeLast(WinMain).ToArray());
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            values.CopyTo(result, count - WinMain);
            return result;
        }
        
    }
}