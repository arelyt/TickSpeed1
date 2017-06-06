using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Ускорение объема сделок на покупку/продажу в секунду в квадрате.
    [HandlerCategory("Arelyt")]
    [HandlerName("VolAccel")]
    public class Vaccel : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Направление", NotOptimized = true)]
        public TradeDirection Direction { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            if (count < 2)
                return null;
            var values = new double[count];
            var datme = new double[count];
            values[0] = 1;
            for (var i = 1; i < count; i++)
            {
                var trades = security.GetTrades(i);

                datme[i] = TimeSpan.FromTicks(security.Bars[i].Date.Ticks - security.Bars[i - 1].Date.Ticks).TotalSeconds;

                var value = trades.Sum(trd => trd.Direction == Direction ? trd.Quantity : 0);


                //  Проверка на ненулевое время (м.б. ошибка в тиковых данных или их отсутствие. Принудительно делим на 0.1)
                if (datme[i] > 0.0001)
                    values[i] = value / datme[i]*datme[i];
                else
                    values[i] = value / 0.1;
            }
            return values;
        }
    }
}