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
    [HandlerName("CasheSignalNew")]
#pragma warning restore 612
    public class CasheSignalNewClass : ITwoSourcesHandler, ISecurityInput0, IBooleanInput1, IStreamHandler, IBooleanReturns
    {
        
        //[HandlerParameter(Name = "Values", NotOptimized = true)]
        //public V2.Predin Line { get; set; }
        public static IList<double> Tradecashe { get; set; }
        public static IList<bool> Boolcashe { get; set; }
        //public static IList<double> Ncashe { get; set; }

        public IList<bool> Execute(ISecurity sec, IList<bool> bools)
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
            if (Tradecashe.IsNull() || Boolcashe.IsNull())
            {
                
                Tradecashe = tradeno.ToList();
                Boolcashe = bools.ToList();
                //Tcashe = time.ToList();
            }
            else
            {
                var s = Tradecashe.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;

                
                var bl = bools.Skip(count - delta).Take(delta).ToList();
                Boolcashe.AddRange(bl);
                var tr = tradeno.Skip(count - delta).Take(delta).ToList();
                Tradecashe.AddRange(tr);
                //var ti = time.Skip(count - delta).Take(delta).ToList();
                //Tcashe.AddRange(ti);
                
            }

            return Boolcashe.TakeLast(count).ToArray();          

        }
        
    }
}