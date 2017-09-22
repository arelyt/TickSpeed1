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
    [HandlerName("CasheBool2")]
#pragma warning restore 612
    public class CasheSignalBool2Class : ITwoSourcesHandler, ISecurityInput0, IBooleanInput1, IStreamHandler, IBooleanReturns
    {
        
        [HandlerParameter(Name = "Reset", Default = "true", NotOptimized = false)]
        public bool Reset { get; set; }
        private static IList<double> Tradecashe2 { get; set; }
        private static IList<bool> Boolcashe2 { get; set; }
        //public static IList<double> Ncashe { get; set; }

        public IList<bool> Execute(ISecurity sec, IList<bool> bools)
        {
            var count = sec.Bars.Count;
            if (count < 100 || bools.IsNull())
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
            if (Tradecashe2.IsNull() || Boolcashe2.IsNull() || Reset)
            {
                
                Tradecashe2 = tradeno.ToList();
                Boolcashe2 = bools.ToList();
                //Tcashe = time.ToList();
            }
            else
            {
                var s = Tradecashe2.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var bl = bools.Skip(count - delta).Take(delta).ToList();
                Boolcashe2.AddRange(bl);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Tradecashe2.AddRange(tr);
                //var ti = time.Skip(count - delta).Take(delta).ToList();
                //Tcashe.AddRange(ti);
                
            }

            return Boolcashe2.TakeLast(count).ToArray();          

        }
        
    }
}