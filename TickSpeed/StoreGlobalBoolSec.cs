using System.Collections.Generic;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Записываем в глобальные переменные сигналы покупки и продажи
    // от последних минимума и максимума массива соответственно
    // ---------------------------------------------------------
    // Сделано для SiM7 - потом переделать на формирование
    // имени сигнала по произвольному инструменту + идентификатор

    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("StoreBool")]
#pragma warning restore 612

    public class StoreGlobalBoolSec : IDouble2DoubleHandler, IContextUses
    {
        public IContext Context { get; set; }
        public IList<double> Execute(IList<double> myD)
        {
            var ctx = Context;
            var count = myD.Count;
            var values = new double[count];
            if (count == 0)
            {
                return values;
            }
            var signalBuy = myD[count-1] > myD[count - 2] && myD[count - 2] < myD[count - 3];
            var signalSell = myD[count - 1] < myD[count - 2] && myD[count - 2] > myD[count - 3];
            ctx.StoreGlobalObject("SiM7_Buy", signalBuy);
            ctx.StoreGlobalObject("SiM7_Sell", signalSell);
            return values;
        }


        
    }

}
