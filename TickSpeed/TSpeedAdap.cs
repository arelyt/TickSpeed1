using System;
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
    [HandlerName("TSpeedAdaptive")]
#pragma warning restore 612
    public class TspeedAdapClass : ITwoSourcesHandler, IDoubleInput0, IDoubleInput1, IDoubleReturns, IStreamHandler, IValuesHandlerWithNumber, IContextUses
    {
        public IContext Context { set; get; }
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }
        //[HandlerParameter(Name = "Win", NotOptimized = false)]
        //public int Win { get; set; }
        //private double[] _dataCache;
        public IList<double> Execute(ISecurity security, double win)
        {
            var wind = (int)Math.Floor(win);
            var count = security.Bars.Count;
            if (count < wind)
                return null;
            IList<double> values = new double[count];
            if (Direction!=TradeDirection.Unknown)
            {
                for (var i = wind - 1; i < count; i++)
                {
                    var tradelastwin = security.GetTrades(firstBarIndex: i - wind + 1, lastBarIndex: i);
                    values[i] = tradelastwin.Sum(selector: t => t.Direction == Direction ? 1 : 0) / (double) wind;

                }
              
            }
            else
            {
                for (int i = wind - 1; i < count; i++)
                {
                    var tradelastwin = security.GetTrades(firstBarIndex: i - wind + 1, lastBarIndex: i);
                    values[i] = tradelastwin.Count/(double)wind;
                }

               
            }
            values = Series.SMA(values, wind);
            return values;
        }
    }
}