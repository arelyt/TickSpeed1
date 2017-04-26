using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Realtime;

namespace TickSpeed.V2
{
    // Скрипт демонстрирует использование интерфейса ISecurityRt для работы с ордерами.
    public class RealtimeOrder : IExternalScript
    {
        public void Execute(IContext ctx, ISecurity sec)
        {
            var rtSec = sec as ISecurityRt;
            if (rtSec == null)
            {
                ctx.Log("Мы в режиме лаборатории!!!");
                return;
            }
            var buyFlag = (bool) ctx.LoadGlobalObject("SiM7_Buy");
            var sellFlag = (bool)ctx.LoadGlobalObject("SiM7_Sell");

            // Скрипт торговли
            // 
            var currPos = 0D;
                foreach (var order in rtSec.Orders)
                {
                    // В
                    {
                        ctx.Log("Номер бара ");
                        //rtSec.CancelOrder(order);
                    }

                    // Для исполненных ордеров считаем общую позицию по инструменту. Заявка может быть частично исполнена
                    // поэтому учитываем частичное исполнение.
                    if (order.IsExecuted)
                    {
                        currPos += (order.Quantity - order.RestQuantity);
                        ctx.Log("Позиция ==");
                        rtSec.NewOrder(OrderType.Limit, false, (double)rtSec.FinInfo.Ask + 3, 1, "LX");
                    }
                }
            if (buyFlag && !rtSec.HasActiveOrders)
            {
                // Выставим новый ордер на покупку.
                rtSec.NewOrder(OrderType.Limit, true, (double)rtSec.FinInfo.Bid - 3, 1, "LE");
            }

                
            
        }
    }
}
