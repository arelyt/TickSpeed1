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
    [HandlerName("CasheBool3")]
#pragma warning restore 612
    public class CasheSignalBool3Class : ITwoSourcesHandler, ISecurityInput0, IBooleanInput1, IStreamHandler, IBooleanReturns
    {
        
        [HandlerParameter(Name = "Reset", Default = "true", NotOptimized = false)]
        public bool Reset { get; set; }
        private static IList<double> Tradecashe3 { get; set; }
        private static IList<bool> Boolcashe3{ get; set; }
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
            if (Tradecashe3.IsNull() || Boolcashe3.IsNull() || Reset)
            {
                
                Tradecashe3 = tradeno.ToList();
                Boolcashe3 = bools.ToList();
                //Tcashe = time.ToList();
            }
            else
            {
                var s = Tradecashe3.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var bl = bools.Skip(count - delta).Take(delta).ToList();
                Boolcashe3.AddRange(bl);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Tradecashe3.AddRange(tr);
                //var ti = time.Skip(count - delta).Take(delta).ToList();
                //Tcashe.AddRange(ti);
                
            }

            return Boolcashe3.TakeLast(count).ToArray();          

        }
        
    }
}