using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.Script.Handlers;
using TSLab.Script;

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
        public static double[] Tcashe;
        public static double[] Pcashe;
        public static double[] Ncashe;
        public IList<double> Execute(ISecurity sec)
        {
            var count = sec.Bars.Count;
            if (count < 100)
                return null;
            var result = new double[count];
            var price = new double[count];
            var tradeno = new double[count];
            var time = new double[count];
            
            for (var i = 0; i < count; i++)
            {
                tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                price[i] = sec.Bars[i].Close;
                time[i] = sec.Bars[i].Date.TimeOfDay.TotalSeconds - 3600;
                //price[i] = sec.Bars[i].Close;
                //tradeno[i] = sec.Bars[i].FirstTradeId.Number;
                //time[i] = sec.Bars[i].Date.TimeOfDay.TotalSeconds - 36000;
            }
            if (Ncashe.Length == 0 || Tcashe.Length == 0 || Pcashe.Length == 0)
            {
                Array.Copy(tradeno, 0, Ncashe, 0, tradeno.Length);
                Array.Copy(price, 0, Pcashe, 0, price.Length);
                Array.Copy(time, 0, Tcashe, 0, count);
            }
            else
            {
                var s = Ncashe.Last();
                var delta =count - Array.FindIndex(tradeno, 0, w => w.Equals(s)) - 1;
                
                Array.Copy(Ncashe.Skip(delta).Take(count-delta).ToArray(), Ncashe, count-delta);
                Array.Copy(Tcashe.Skip(delta).Take(count - delta).ToArray(), Tcashe, count - delta);
                Array.Copy(Pcashe.Skip(delta).Take(count - delta).ToArray(), Pcashe, count - delta);
                Array.Resize(ref Ncashe, Ncashe.Length + delta);
                Array.Resize(ref Tcashe, Tcashe.Length + delta);
                Array.Resize(ref Pcashe, Pcashe.Length + delta);
                for (int i = count - delta; i < count - 1; i++)
                {
                    Ncashe[i] = tradeno[i];
                    Tcashe[i] = time[i];
                    Pcashe[i] = price[i];
                }
            }

            result = Pcashe;          
            return result;
        }

        
    }
}