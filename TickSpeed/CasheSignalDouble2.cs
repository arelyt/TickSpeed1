﻿using System;
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
    [HandlerName("CasheSignalDouble2")]
#pragma warning restore 612
    public class CasheSignalDouble2Class : ITwoSourcesHandler, ISecurityInput0, IDoubleInput1, IStreamHandler, IDoubleReturns
    {
        
        [HandlerParameter(Name = "Values", Default = "true", NotOptimized = false)]
        public bool Reset { get; set; }
        private static IList<double> Tradecashe1 { get; set; }
        private static IList<double> Doublecashe2 { get; set; }
        //public static IList<double> Ncashe { get; set; }

        public IList<double> Execute(ISecurity sec, IList<double> bools)
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
            if (Doublecashe2.IsNull() || Doublecashe2.IsNull() || Reset)
            {
                
                Tradecashe1 = tradeno.ToList();
                Doublecashe2 = bools.ToList();
                //Tcashe = time.ToList();
            }
            else
            {
                var s = Tradecashe1.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var bl = bools.Skip(count - delta).Take(delta).ToList();
                Doublecashe2.AddRange(bl);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Tradecashe1.AddRange(tr);
                //var ti = time.Skip(count - delta).Take(delta).ToList();
                //Tcashe.AddRange(ti);
                
            }

            return Doublecashe2.TakeLast(count).ToArray();          

        }
        
    }
}