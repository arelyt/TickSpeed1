using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

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

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            //var datme = new double[count];
            var time = new double[count];
            values[0] = 1;
            for (var i = 1; i < count; i++)
            {
                var tradelast = security.GetTrades(i);
                //var tradeprev = security.GetTrades(i-1);

                //datme[i] = TimeSpan.FromTicks(security.Bars[i].Date.Ticks - security.Bars[i-1].Date.Ticks).TotalSeconds;
                //time[i] = (tradelast[0].Date.TimeOfDay.TotalSeconds - tradeprev[0].Date.TimeOfDay.TotalSeconds);
                time[i] = (tradelast.Last().Date.TimeOfDay.TotalSeconds - tradelast.First().Date.TimeOfDay.TotalSeconds);
                var value = tradelast.Sum(t => t.Direction == Direction ? 1 : 0);

                //  Проверка на ненулевое время (м.б. ошибка в тиковых данных или их отсутствие. Принудительно делим на 0.1)
                if (time[i] > 0.0001)
                    values[i] = value / time[i];
                else
                    values[i] = value / 0.0001;
                
                
            }
            return values;
        }
    }
}