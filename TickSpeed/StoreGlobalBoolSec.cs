using System.Collections.Generic;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Записываем в глобальную переменую сигнал покупки
    // от последнего минимума массива
    // ---------------------------------------------------------
    // Сделано для SiM7 - потом переделать на формирование
    // имени сигнала по произвольному инструменту + идентификатор

    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("StoreBool")]
#pragma warning restore 612

    public class StoreGlobalBoolSec : IContextUses, IDoubleInputs, IBooleanReturns
    {
        public IContext Context { get; set; }
        public bool Execute(IList<double> myD)
        {
            var count = myD.Count;
            var values = myD[count] > myD[count - 1] && myD[count - 1] < myD[count - 2];
            Context.StoreGlobalObject("SiM7_Buy", values);
            return values;
        }

        
    }

}
