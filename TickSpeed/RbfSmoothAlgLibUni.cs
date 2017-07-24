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

        public static IContext Ctx { set; get; }
        private readonly TSLab.Script.Handlers.Bid _bidh = new TSLab.Script.Handlers.Bid {Context = Ctx};

        private readonly TSLab.Script.Handlers.Ask _askh = new TSLab.Script.Handlers.Ask {Context = Ctx};

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
            var bid = _bidh.Execute(security);
            var ask = _askh.Execute(security);
            var xy = new double[count, 3];
            // string rfunc;
            switch (Method)
            {
                case V2.RbfAlgLibMethodOfInput.Close:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = security.Bars[i].Close;
                        time[i] = i;
                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.Ask:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = ask[i];
                        time[i] = i;

                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.Bid:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = bid[i];
                        time[i] = i;

                    }
                    break;
                case V2.RbfAlgLibMethodOfInput.HalfBidAsk:
                    
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = (ask[i] + bid[i])/2;
                        time[i] = i;

                    }
                    break;
                default:
                    for (int i = 0; i < count; i++)
                    {
                        xy[i, 0] = i;
                        xy[i, 2] = security.Bars[i].Close;
                        time[i] = i;
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