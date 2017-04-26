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
    [HandlerName("LoadBool")]
#pragma warning restore 612

    public class LoadGlobalBoolSec : ISecurityInputs, IContextUses, IBooleanReturns
    {
        public IContext Context { get; set; }
        public bool Execute(ISecurity sec)
        {
            var values = (bool)Context.LoadGlobalObject("SiM7_Buy");
            return values;
        }

        
    }

}
