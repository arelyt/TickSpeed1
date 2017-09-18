using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script;
//using TSLab.Script;
using TSLab.Script.Handlers;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmoothAlgLibDouble")]
#pragma warning restore 612
    public class RbfSmoothAlgLibDoubleClass : ITwoSourcesHandler, ISecurityInput0,
                                                IDoubleInput1, IStreamHandler, IDoubleReturns, IContextUses
    {
        [HandlerParameter(Name = "NLayer", Default = "3", NotOptimized = false)]
        public int Nlayer { get; set; }
        [HandlerParameter(Name = "Smooth", Default = "0.01", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", Default = "2.0", NotOptimized = false)]
        public double Rbfconst { get; set; }
        [HandlerParameter(Name = "TimeInput", Default = "false", NotOptimized = true)]
        public bool Timeinput { get; set; }

        private rbfmodel _model;
        public IContext Context { get; set; }


        public IList<double> Execute(ISecurity security, IList<double> md)
        {
            var t = DateTime.Now;
            var ctx = Context;
            var count = md.Count;
            if (count < 10)
                return null;
            rbfcreate(2, 1, out _model);
            // var v = alglib.rbfcalc2(model, 0.0, 0.0);
            var result = new double[count];
            var values = new double[count];
            var time = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = md[i];
                //time[i] = i;
            }
            var xy = new double[count, 3];
            for (var i = 0; i < count; i++)
            {
                //xy[i, 0] = time[i];
                xy[i, 2] = values[i];
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
            rbfsetpoints(_model, xy);
            // v = alglib.rbfcalc2(model, 0.0, 0.0);
            // ReSharper disable once NotAccessedVariable
            rbfreport rep;
            rbfsetalgohierarchical(_model, Rbfconst, Nlayer, Smooth);
            //rbf.rbfset(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            for (var i = 0; i < count; i++)
            {
                result[i] = rbfcalc2(_model, time[i], 0.0);
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            ctx.Log("rbf exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}