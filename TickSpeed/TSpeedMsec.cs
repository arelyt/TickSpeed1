using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using TickSpeed.V2;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using Series = TSLab.Script.Helpers.Series;

namespace TickSpeed
{
    // Скорость сделок на покупку/продажу в секунду.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("TSpeedMsec")]
#pragma warning restore 612
    public class TspeedMsec : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        [HandlerParameter(true, "2", Name = "Win", NotOptimized = false)]
        public int Win { get; set; }
        [HandlerParameter(true, "5", Name = "WinExp", NotOptimized = false)]
        public int WinExp{ get; set; }
        [HandlerParameter(true, "0.01", Name = "K", NotOptimized = false)]
        public double K { get; set; }
        [HandlerParameter(Name = "Output", Default = "0", NotOptimized = true)]
        public OutputMethod Method { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            IList<double> values = new double[count];
            var nbuy = new double[count];
            var nsell = new double[count];
            switch (Method)
            {
                case OutputMethod.Simple:
                        if (Direction != TradeDirection.Unknown)
                        {
                            for (var i = Win - 1; i < count; i++)
                            {
                                var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                                values[i] = tradelastwin.Sum(selector: t => t.Direction == Direction ? 1 : 0) / (double)Win;

                            }

                        }
                        else
                        {
                            for (int i = Win - 1; i < count; i++)
                            {
                                var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                                values[i] = tradelastwin.Count / (double)Win;
                            }


                        }
                        break;
                case OutputMethod.Osc:

                    for (var i = Win - 1; i < count; i++)
                    {
                        var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                        nbuy[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Buy ? 1 : 0) / (double)Win;
                        nsell[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Sell ? 1 : 0) / (double)Win;
                    }
                    nbuy = (double[])Series.EMA(nbuy, WinExp);
                    nsell = (double[])Series.EMA(nsell, WinExp);
                    for (int i = 0; i < count; i++)
                    {
                        values[i] = (nbuy[i] - nsell[i]) / (nbuy[i] + nsell[i]);
                    }
                    
                    break;
                case OutputMethod.OscTanh:
                    
                    for (var i = Win - 1; i < count; i++)
                    {
                        var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                        nbuy[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Buy ? 1 : 0) / (double)Win;
                        nsell[i] = tradelastwin.Sum(selector: t => t.Direction == TradeDirection.Sell ? 1 : 0) / (double)Win;
                    }
                    nbuy = (double[])Series.EMA(nbuy, WinExp);
                    nsell = (double[])Series.EMA(nsell, WinExp);
                    for (int i = 0; i < count; i++)
                    {
                        values[i] = Math.Tanh(K * Math.Log(nbuy[i] + nsell[i]) * (nbuy[i] - nsell[i]));
                    }
                    
                    break;
            }
            
            values = Series.EMA(values, WinExp);
            for (int i = 0; i < count; i++)
            {
                if (values[i].IsNaN())
                {
                    values[i] = 0;
                }
            }
            return values;
        }
    }
}