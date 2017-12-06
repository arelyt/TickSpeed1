using System.Collections.Generic;
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

        public IList<double> Execute(ISecurity sec)
        {
            var ctx = Context;
            //var values = new double[sec.Bars.Count];
            var values = (IList<double>)ctx.LoadObject(Objname);
            return values;
        }

        
    }

}
