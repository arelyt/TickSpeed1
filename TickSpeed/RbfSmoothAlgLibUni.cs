using System.Collections.Generic;
using TickSpeed.V2;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.DataSource;
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

        private rbfmodel _model;

        public IContext Context { set; get; }
        private TSLab.Script.Handlers.Bid Bid_h = new TSLab.Script.Handlers.Bid();

        private TSLab.Script.Handlers.Ask Ask_h = new TSLab.Script.Handlers.Ask();

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 10)
                return null;
            rbfcreate(2, 1, out _model);
            // var v = alglib.rbfcalc2(model, 0.0, 0.0);
            var result = new double[count];
            //var values = new double[count];
            var time = new double[count];
            var xy = new double[count, 3];
            // string rfunc;
            switch (Method)
            {
                case V2.RbfAlgLibMethodOfInput.Close:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = security.Bars[i].Close;
                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.Ask:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = Ask_h.Execute(security)[i];
                      
                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.Bid:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = Bid_h.Execute(security)[i];

                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.HalfBidAsk:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = (Ask_h.Execute(security)[i] + Bid_h.Execute(security)[i])/2;

                    }
                    break;
                default:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = security.Bars[i].Close;
                    }
                    break;

            }
            //for (var i = 0; i < count; i++)
            //{
            //    values[i] = security.Bars[i].Close;
            //    time[i] = i;
            //}
            
            //for (var i = 0; i < count; i++)
            //{
            //    xy[i, 0] = time[i];
            //    xy[i, 2] = values[i];
            //}
            rbfsetpoints(_model, xy);
            // v = alglib.rbfcalc2(model, 0.0, 0.0);
            rbfreport rep;
            rbfsetalgohierarchical(_model, Rbfconst, Nlayer, Smooth);
            //rbf.rbfset(_model, Rbfconst, Nlayer, Smooth);
            rbfbuildmodel(_model, out rep);
            for (var i = 0; i < count; i++)
            {
                result[i] = rbfcalc2(_model, time[i], 0.0);
            }
            return result;
        }
    }
}