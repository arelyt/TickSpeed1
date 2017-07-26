using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using RusAlgo.Helper;
using TickSpeed.V2;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Utils;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSCashe")]
#pragma warning restore 612
    public class RbfSmoothAlgLibUniCasheClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "NLayer", Default = "3", NotOptimized = false)]
        public int Nlayer { get; set; }
        [HandlerParameter(Name = "Smooth", Default = "0.01", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", Default = "2.0", NotOptimized = false)]
        public double Rbfconst { get; set; }
        [HandlerParameter(Name = "MethodInput", Default = "0", NotOptimized = true)]
        public RbfAlgLibMethodOfInput Method { get; set; }
        [HandlerParameter(Name = "TimeInput", Default = "false", NotOptimized = true)]
        public bool Timeinput { get; set; }
        [HandlerParameter(Name = "WinCalc", Default = "500", NotOptimized = false)]
        public int WinCalc { get; set; }

        private rbfmodel _model;

        public static IContext Ctx { set; get; }
        public static IList<double> IndiCashe { set; get; }
        public static IList<double> Ncashe { set; get; }
        public static IList<double> Tcashe { set; get; }
        private readonly TSLab.Script.Handlers.Bid _bidh = new TSLab.Script.Handlers.Bid {Context = Ctx};

        private readonly TSLab.Script.Handlers.Ask _askh = new TSLab.Script.Handlers.Ask {Context = Ctx};

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < WinCalc)
                return null;
            rbfcreate(2, 1, out _model);
            var result = new double[count];
            var tradeno = new double[count];
            var time = new double[count];
            var price = new double[count];
            var bid = _bidh.Execute(security);
            var ask = _askh.Execute(security);
            var xy = new double[WinCalc, 3];

            for (var i = 0; i < count; i++)
            {
                tradeno[i] = security.Bars[i].FirstTradeId.Number;
                time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds;
                price[i] = security.Bars[i].Close;

            }

            if (Ncashe.IsNull() || Tcashe.IsNull() || IndiCashe.IsNull())
            {

                Ncashe = tradeno.ToList();
                IndiCashe = price.ToList();
                Tcashe = time.ToList();
            }
            else
            {
                var s = Ncashe.Last();
                var delta = count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;


                var pr = price.Skip(count - delta).Take(delta).ToList();
                IndiCashe.AddRange(pr);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Ncashe.AddRange(tr);
                var ti = time.Skip(count - delta).Take(delta).ToList();
                Tcashe.AddRange(ti);
            }
            var tr = tradeno.TakeLast(WinCalc).ToList();
            var ti = time.TakeLast(WinCalc).ToList();
            var pr = price.TakeLast(WinCalc).ToList();
            
            switch (Method)
            {
                case RbfAlgLibMethodOfInput.Close:
                    for (var i = 0; i < WinCalc; i++)
                    {
                        xy[i, 2] = pr[i];
                        if (Timeinput)
                        {
                            xy[i, 0] = ti[i];
                        }
                        else
                        {
                            xy[i, 0] = i;
                        }
                        
                    }
                    break;
                case RbfAlgLibMethodOfInput.Ask:
                    for (var i = 0; i < WinCalc; i++)
                    {
                        xy[i, 2] = ask[i];
                        if (Timeinput)
                        {
                            xy[i, 0] = ti[i];
                        }
                        else
                        {
                            xy[i, 0] = i;
                        }
                        
                    }
                    break;
                case RbfAlgLibMethodOfInput.Bid:
                    for (var i = 0; i < WinCalc; i++)
                    {
                        xy[i, 2] = bid[i];
                        if (Timeinput)
                        {
                            xy[i, 0] = ti[i];
                        }
                        else
                        {
                            xy[i, 0] = i;
                        }

                    }
                    break;
                case RbfAlgLibMethodOfInput.HalfBidAsk:
                    
                    for (var i = 0; i < WinCalc; i++)
                    {
                        xy[i, 2] = (ask[i] + bid[i])/2;
                        if (Timeinput)
                        {
                            xy[i, 0] = ti[i];
                        }
                        else
                        {
                            xy[i, 0] = i;
                        }
                    }
                    break;
                default:
                    for (var i = 0; i < WinCalc; i++)
                    {
                        xy[i, 2] = pr[i];
                        if (Timeinput)
                        {
                            xy[i, 0] = ti[i];
                        }
                        else
                        {
                            xy[i, 0] = i;
                        }
                    }
                    break;

            }
            
            rbfsetpoints(_model, xy);
            rbfreport rep;
            rbfsetalgohierarchical(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            for (var i = 0; i < WinCalc; i++)
            {
                if (Timeinput)
                {
                    result[i] = rbfcalc2(_model, ti[i], 0.0);
                }
                else
                {
                    result[i] = rbfcalc2(_model, i, 0.0);
                }
                
            }
            return result;
        }
    }
}