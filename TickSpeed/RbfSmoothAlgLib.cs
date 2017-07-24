using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("RBFSmoothAlgLib")]
#pragma warning restore 612
    public class RbfSmoothAlgLibClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "NLayer", Default = "3", NotOptimized = false)]
        public int Nlayer { get; set; }
        [HandlerParameter(Name = "Smooth", Default = "0.01", NotOptimized = false)]
        public double Smooth { get; set; }
        [HandlerParameter(Name = "RbfConst", Default = "2.0", NotOptimized = false)]
        public double Rbfconst { get; set; }

        private rbfmodel _model;
        

        
        public IList<double> Execute(ISecurity security)
        {
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
            var xy = new double[count, 3];
            for (var i = 0; i < count; i++)
            {
                xy[i, 0] = time[i];
                xy[i, 2] = values[i];
            }
            rbfsetpoints(_model, xy);
            // v = alglib.rbfcalc2(model, 0.0, 0.0);
            rbfreport rep;
            rbfsetalgomultilayer(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            for (var i = 0; i < count; i++)
            {
                result[i] = rbfcalc2(_model, time[i], 0.0);
            }
            return result;
        }
    }
}