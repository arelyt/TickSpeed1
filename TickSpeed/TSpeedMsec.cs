using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
    // Скорость сделок на покупку/продажу в секунду.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("TSpeedMsec")]
#pragma warning restore 612
    public class TspeedMsec : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        [HandlerParameter(true, "2", Name = "Win", NotOptimized = false)]
        public int Win { get; set; }
        [HandlerParameter(true, "5", Name = "WinExp", NotOptimized = false)]
        public int WinExp{ get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            IList<double> values = new double[count];
            if (Direction!=TradeDirection.Unknown)
            {
                for (var i = Win - 1; i < count; i++)
                {
                    var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                    values[i] = tradelastwin.Sum(selector: t => t.Direction == Direction ? 1 : 0) / (double) Win;

                }
              
            }
            else
            {
                for (int i = Win - 1; i < count; i++)
                {
                    var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                    values[i] = tradelastwin.Count/(double)Win;
                }

               
            }
            values = Series.EMA(values, WinExp);
            return values;
        }
    }
}