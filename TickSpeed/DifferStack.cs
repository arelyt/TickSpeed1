using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using TSLab.Script.Handlers;
using vvTSLtools;

namespace TickSpeed
{
    // Дифференциал
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("DiffStack")]
#pragma warning restore 612
    public class DifferStackClass : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context {get; set;}
        [HandlerParameter(true, "stack", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }
        [HandlerParameter(true, Name = "Diff", Default = "true", NotOptimized = false)]
        public bool Diff { get; set; }
        [HandlerParameter(true, Name = "Reset", Default = "false", NotOptimized = false)]
        public bool Reset { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var ctx = Context;
            var count = myDoubles.Count;
            if (count < 100 || Reset)
                return myDoubles;
            var values = new double[count];
            values[0] = 0;
            if (Diff)
            {
                for (var i = 1; i < count; i++)
                {
                    values[i] = myDoubles[i] - myDoubles[i - 1];
                }
            }
            else
            {
                for (var i = 1; i < count; i++)
                {
                    values[i] = myDoubles[i];
                }
            }
            if (ctx.LoadObject(Objname).IsNull())
            {
                ctx.StoreObject(Objname, values);
                var val = (double[])ctx.LoadObject(Objname);
                return val.ToList();
            }
            else
            {
                var val = (double[])ctx.LoadObject(Objname);
                var delta = values.Length - val.Length;
                var bl = values.Skip(count - delta).Take(delta).ToList();
                var temp = val.ToList();
                temp.AddRange(bl);
                //if (_val.Length != count)
                //{
                //    Array.Resize(ref _val, count);
                //}
                //Array.Copy(_val, values, _val.Length);
                ctx.StoreObject(Objname, temp.TakeLast(count).ToArray());
                return temp.ToList();
            }
            
        }
        
    }

}
