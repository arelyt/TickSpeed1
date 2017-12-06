using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    [HandlerName("LoadLocalDouble")]
#pragma warning restore 612

    public class LoadLocalObjectClass : IOneSourceHandler, ISecurityInputs,
                                        IDoubleReturns, IStreamHandler, IContextUses 
    { 
        public IContext Context { get; set; }
        public IList<double> Execute(ISecurity sec)
        {
            var ctx = Context;
            var values = new double[ctx.BarsCount];
            values = (double[])ctx.LoadObject("fore1");
            return values.ToList();
        }

        
    }

}
