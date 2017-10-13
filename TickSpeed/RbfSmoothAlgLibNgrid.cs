using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoreLinq;
using TSLab.Script;
using TSLab.Script.Handlers;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmoothAlgLibNgrid")]
#pragma warning restore 612
    public class RbfSmoothAlgLibNgridClass : IOneSourceHandler, ISecurityInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        [HandlerParameter(Name = "NLayer", Default = "3", NotOptimized = false)]
        public int Nlayer { get; set; }
        [HandlerParameter(Name = "Smooth", Default = "0.01", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", Default = "2.0", NotOptimized = false)]
        public double Rbfconst { get; set; }

        private rbfmodel _model;
        public IContext Context { get; set; }


        public IList<double> Execute(ISecurity security)
        {
            var t = DateTime.Now;
            var count = security.Bars.Count;
            if (count < 10)
                return null;
            rbfcreate(2, 1, out _model);
            // var v = alglib.rbfcalc2(model, 0.0, 0.0);
            var result = new double[count];
            var values = new double[count];
            var time = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = security.Bars[i].Close;
                time[i] = i;
            }
            var ma = values.Max();
            var mi = values.Min();
            var ngridcount = (int)((ma - mi)/security.Tick + 1.0);
            var ngrid = new double[ngridcount];
            ngrid[0] = mi;
            for (var i = 1; i < ngridcount; i++)
            {
                ngrid[i] = ngrid[i-1] + security.Tick;
            }
            var xy = new double[count, 3];
            for (var i = 0; i < count; i++)
            {
                xy[i, 0] = time[i];
                xy[i, 2] = values[i];
            }
            rbfsetpoints(_model, xy);
            // v = alglib.rbfcalc2(model, 0.0, 0.0);
            rbfreport rep;
            rbfsetalgohierarchical(_model, Rbfconst, Nlayer, Smooth);
            //rbf.rbfset(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            alglib.smp_rbfgridcalc2v(_model, time, count, ngrid, ngrid.Length, out result);
            //for (var i = 0; i < count; i++)
            //{
            //    result[i] = rbfcalc2(_model, time[i], 0.0);
            //}
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("RBF exec for " + g + " msec", MessageType.Info, toMessageWindow: true);
            return result;
        }
    }
}