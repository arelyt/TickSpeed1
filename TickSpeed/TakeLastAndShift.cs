using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using RusAlgo.Helper;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
    [HandlerName("TakeLastAndShiftLeft")]
    public class TakeLastAndShiftLeftClass : ITwoSourcesHandler, ISecurityInput0 , IDoubleInput1, IDoubleReturns, IStreamHandler, IContextUses
    {

        // Пока без шифта
        [HandlerParameter(true, "0", Name = "LShift", Max = "20", Min = "0", Step = "1", NotOptimized = false)]
        public int ShiftL { get; set; }
        [HandlerParameter(true, "1", Name = "Take", Max = "20", Min = "0", Step = "1", NotOptimized = false)]
        public int Take { get; set; }
        [HandlerParameter(true, "fore", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }

        public IList<double> Execute(ISecurity security, IList<double> myDoubles)
        {
            var ctx = Context;
            var seccount = security.Bars.Count;
            var count = myDoubles.Count;
            var delta = count - seccount;
            IList<double> l;
            if (count < 10)
                return null;
            if (ctx.LoadObject(Objname).IsNull())
            {
                ctx.StoreObject(Objname, myDoubles);
            }
            else
            {
                
            }
            l = (IList<double>)ctx.LoadObject(Objname);
            var temp = l.TakeLast(Take).ToArray();
            var result = myDoubles.ToList().Add(temp).TakeLast(count);
            ctx.StoreObject(Objname, result);
            return result.ToList();
        }

        public IContext Context { get; set; }
    }

}
