using System;
using System.Collections.Generic;
using System.Globalization;
using TickSpeed.V2;
using TSLab.Script;
using TSLab.Script.Handlers;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmoothAlgLibUni")]
#pragma warning restore 612
    public class RbfSmoothAlgLibUniClass : IBar2DoubleHandler
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

        private rbfmodel _model;

        public static IContext Ctx { set; get; }
        private readonly TSLab.Script.Handlers.Bid _bidh = new TSLab.Script.Handlers.Bid {Context = Ctx};

        private readonly TSLab.Script.Handlers.Ask _askh = new TSLab.Script.Handlers.Ask {Context = Ctx};

        public IList<double> Execute(ISecurity security)
        {
            var t = DateTime.Now;
            var count = security.Bars.Count;
            if (count < 10)
                return null;
            rbfcreate(2, 1, out _model);
            var result = new double[count];
            var time = new double[count];
            var bid = _bidh.Execute(security);
            var ask = _askh.Execute(security);
            var xy = new double[count, 3];
            
            switch (Method)
            {
                case RbfAlgLibMethodOfInput.Close:
                    for (var i = 0; i < count; i++)
                    {
                        
                        xy[i, 2] = security.Bars[i].Close;
                        if (Timeinput)
                        {
                            time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds -
                                      security.Bars[0].Date.TimeOfDay.TotalSeconds;
                        }
                        else
                        {
                            time[i] = i;
                        }
                        xy[i, 0] = time[i];

                    }
                    break;
                case RbfAlgLibMethodOfInput.Ask:
                    for (var i = 0; i < count; i++)
                    {
                        
                        xy[i, 2] = ask[i];
                        if (Timeinput)
                        {
                            time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds -
                                      security.Bars[0].Date.TimeOfDay.TotalSeconds;
                        }
                        else
                        {
                            time[i] = i;
                        }
                        xy[i, 0] = time[i];

                    }
                    break;
                case RbfAlgLibMethodOfInput.Bid:
                    for (var i = 0; i < count; i++)
                    {
                        
                        xy[i, 2] = bid[i];
                        if (Timeinput)
                        {
                            time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds -
                                      security.Bars[0].Date.TimeOfDay.TotalSeconds;
                        }
                        else
                        {
                            time[i] = i;
                        }
                        xy[i, 0] = time[i];

                    }
                    break;
                case RbfAlgLibMethodOfInput.HalfBidAsk:
                    
                    for (var i = 0; i < count; i++)
                    {
                        
                        xy[i, 2] = (ask[i] + bid[i])/2;
                        if (Timeinput)
                        {
                            time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds -
                                      security.Bars[0].Date.TimeOfDay.TotalSeconds;
                        }
                        else
                        {
                            time[i] = i;
                        }
                        xy[i, 0] = time[i];

                    }
                    break;
                default:
                    for (var i = 0; i < count; i++)
                    {
                        
                        xy[i, 2] = security.Bars[i].Close;
                        if (Timeinput)
                        {
                            time[i] = security.Bars[i].Date.TimeOfDay.TotalSeconds -
                                      security.Bars[0].Date.TimeOfDay.TotalSeconds;
                        }
                        else
                        {
                            time[i] = i;
                        }
                        xy[i, 0] = time[i];
                    }
                    break;

            }
            
            rbfsetpoints(_model, xy);
            rbfreport rep;
            rbfsetalgohierarchical(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            for (var i = 0; i < count; i++)
            {
                result[i] = rbfcalc2(_model, time[i], 0.0);
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            //Context.Log("rbf exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}