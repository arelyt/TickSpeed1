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
    [HandlerName("LoadBoolBuy")]
#pragma warning restore 612

    public class LoadGlobalBoolBuy : IBar2BoolHandler, IContextUses
    {
        public IContext Context { get; set; }
        public bool Execute(ISecurity sec, int barNum)
        {
            var ctx = Context;
            var values = (bool)ctx.LoadGlobalObject("SiM7_Buy");
            return values;
        }


        
    }

}
