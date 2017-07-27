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
    // Cashe bool signal
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("CasheSignalDouble")]
#pragma warning restore 612
    public class CasheSignalDoubleClass : ITwoSourcesHandler, ISecurityInput0, IDoubleInput1, IStreamHandler, IDoubleReturns
    {
        
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }
        public static IList<double> Tradecashe1 { get; set; }
        public static IList<double> Doublecashe { get; set; }
        //public static IList<double> Ncashe { get; set; }

        public IList<double> Execute(ISecurity sec, IList<double> bools)
        {
            var count = sec.Bars.Count;
            if (count < 100)
                return null;
            //var result = new double[count];
            //var price = new double[count];
            var tradeno = new double[count];
            //var time = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                //price[i] = sec.Bars[i].Close;
                //time[i] = sec.Bars[i].Date.TimeOfDay.TotalSeconds;
                
            }
            if (Doublecashe.IsNull() || Doublecashe.IsNull())
            {
                
                Tradecashe1 = tradeno.ToList();
                Doublecashe = bools.ToList();
                //Tcashe = time.ToList();
            }
            else
            {
                var s = Tradecashe1.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var bl = bools.Skip(count - delta).Take(delta).ToList();
                Doublecashe.AddRange(bl);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Tradecashe1.AddRange(tr);
                //var ti = time.Skip(count - delta).Take(delta).ToList();
                //Tcashe.AddRange(ti);
                
            }

            return Doublecashe.TakeLast(count).ToArray();          

        }
        
    }
}