using System.Collections.Generic;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Считываем из локального кэша
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("LoadLocalDouble")]
#pragma warning restore 612

    public class LoadLocalObjectClass : IOneSourceHandler, ISecurityInputs,
                                        IDoubleReturns, IStreamHandler, IContextUses 
    { 
        public IContext Context { get; set; }
        [HandlerParameter(true, "fore", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }

        [HandlerParameter(true, "5", Name = "Order", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Order { get; set; }

        [HandlerParameter(true, "61", Name = "winSG", Max = "120", Min = "1", Step = "1", NotOptimized = false)]
        public int WinSg { get; set; }

        [HandlerParameter(true, "0", Name = "deriv", Max = "3", Min = "0", Step = "1", NotOptimized = true)]
        public int Deriv { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            var ctx = Context;
            //var values = new double[sec.Bars.Count];
            var values = (double[])ctx.LoadObject(Objname);
            var result = new double[values.Length];
            SavitzkyGolay sg = new SavitzkyGolay(WinSg, Deriv, Order);
            sg.Apply(values, result);
            return result;
        }

        
    }

}
