using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Считываем из в глобальной переменной сигнал покупки
    // от последнего минимума массива
    // ---------------------------------------------------------
    // Сделано для SiM7 - потом переделать на формирование
    // имени сигнала по произвольному инструменту + идентификатор

    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("LoadBoolBuySWT")]
#pragma warning restore 612

    public class LoadGlobalBoolBuySWT : IOneSourceHandler, IContextUses,
                                        IStreamHandler, IBooleanReturns
    {
        public IContext Context { get; set; }
        public IList<bool> Execute(ISecurity sec)
        {
            var ctx = Context;
            var values = new bool[ctx.BarsCount];
            values[ctx.BarsCount-1] = (bool)ctx.LoadGlobalObject("SiM7TICK64SWT_Buy");
            return values;
        }


        
    }

}
