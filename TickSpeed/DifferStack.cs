using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using TSLab.Script.Handlers;
using TSLab.Utils;
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

        public IList<double> Execute(IList<double> myDoubles)
        {
            var ctx = Context;
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            values[0] = 0;

            for (var i = 1; i < count; i++)
            {
                values[i] = myDoubles[i] - myDoubles[i - 1];
            }
            double[] _val = (double[])ctx.LoadObject(Objname);
            if (_val.IsNull())
            {
                ctx.StoreObject(Objname, values);
                _val = values;
            }
            else
            {
                var delta = values.Length - _val.Length;
                var bl = values.Skip(count - delta).Take(delta).ToList();
                _val.AddRange(bl);
                //if (_val.Length != count)
                //{
                //    Array.Resize(ref _val, count);
                //}
                //Array.Copy(_val, values, _val.Length);
                ctx.StoreObject(Objname, _val.TakeLast(count));
                
            }
            return _val.ToList();
        }
        
    }

}
