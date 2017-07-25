using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using RusAlgo.Helper;
using TSLab.Script.Handlers;
using TSLab.Script;
using TSLab.Utils;

namespace TickSpeed
{
    // Cashe
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CasheSignal")]
#pragma warning restore 612
    public class CasheSignalClass : IBar2DoubleHandler
    {
        
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }
        public static IList<double> Tcashe { get; set; }
        public static IList<double> Pcashe { get; set; }
        public static IList<double> Ncashe { get; set; }

        public IList<double> Execute(ISecurity sec)
        {
            var count = sec.Bars.Count;
            if (count < 100)
                return null;
            //var result = new double[count];
            var price = new double[count];
            var tradeno = new double[count];
            var time = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                price[i] = sec.Bars[i].Close;
                time[i] = sec.Bars[i].Date.TimeOfDay.TotalSeconds;
                
            }
            if (Ncashe.IsNull() || Tcashe.IsNull() || Pcashe.IsNull())
            {
                
                Ncashe = tradeno.ToList();
                Pcashe = price.ToList();
                Tcashe = time.ToList();
            }
            else
            {
                var s = Ncashe.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var pr = price.Skip(count - delta).Take(delta).ToList();
                Pcashe.AddRange(pr);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Ncashe.AddRange(tr);
                var ti = time.Skip(count - delta).Take(delta).ToList();
                Tcashe.AddRange(ti);
                
            }

            return Pcashe.TakeLast(count).ToArray();          

        }
        
    }
}