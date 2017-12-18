using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.DataSource;

namespace TickSpeed
{
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Spread")]
#pragma warning restore 612
    public class SpreadClass : ISecurityInputs, ISecurityReturns, ITwoSourcesHandler, IStreamHandler
    {
           public ISecurity Execute(ISecurity source1, ISecurity source2)
        {
            var bars = new List<DataBar>(source1.Bars.Count);
            for (int i = 0; i < source1.Bars.Count; i++)
            {
            	var bar = source1.Bars[i];
            	var o1 = source2.Bars[i].Open;
            	var h1 = source2.Bars[i].High;
            	var l1 = source2.Bars[i].Low;
            	var c1 = source2.Bars[i].Close;
                
                var newBar = new DataBar(bar.Date, bar.Open-o1,
                                     bar.High-h1,
                                     bar.Low-l1,
                                     bar.Close-c1,
                                     bar.Volume);
            	bars.Add(newBar);
            }
            return source1.CloneAndReplaceBars(bars);
        }
    }
}