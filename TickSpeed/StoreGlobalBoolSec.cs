using System;
using System.Collections.Generic;
using TSLab.Script;
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

    public class StoreGlobalBoolSec : IContextUses, IOneSourceHandler, IDoubleInputs
    {
        public IContext Context { get; set; }
        public IList<double> Execute(ISecurity sec, IList<double> myD, IList<double> myD1)
        {
            var header = sec.Symbol + sec.IntervalBase + sec.Interval;
            var ctx = Context;
            var count = myD.Count;
            var count1 = myD1.Count;
            var values = new double[count];
            if (count == 0 || count != count1)
            {
                return values;
            }
            var signalBuySWT = myD[count-1] > myD[count - 2] && myD[count - 2] < myD[count - 3];
            var signalSellSWT = myD[count - 1] < myD[count - 2] && myD[count - 2] > myD[count - 3];
            var signalBuyTSpeed = myD1[count1] < 0 && myD1[count1 - 1] > 0;
            var signalSellTSpeed = myD1[count1] > 0 && myD1[count1 - 1] < 0;
            ctx.StoreGlobalObject(header + "SWT_Buy", signalBuySWT);
            ctx.StoreGlobalObject(header + "SWT_Sell", signalSellSWT);
            ctx.StoreGlobalObject(header + "TSpeed_Buy", signalBuyTSpeed);
            ctx.StoreGlobalObject(header + "TSpeed_Sell", signalSellTSpeed);
            return values;
        }

        
    }

}
