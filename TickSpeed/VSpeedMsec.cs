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
    [HandlerName("VSpeedMsec")]
#pragma warning restore 612
    public class VspeedMsec : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        [HandlerParameter(Name = "Win", NotOptimized = false)]
        public int Win { get; set; }

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
                    values[i] = tradelastwin.Sum(selector: t => t.Direction == Direction ? t.Quantity : 0) / (double) Win;

                }
              
            }
            else
            {
                for (int i = Win - 1; i < count; i++)
                {
                    var tradelastwin = security.GetTrades(firstBarIndex: i - Win + 1, lastBarIndex: i);
                    values[i] = tradelastwin.Sum(t => t.Volume)/(double)Win;
                }

               
            }
            values = Series.SMA(values, Win);
            return values;
        }
    }
}