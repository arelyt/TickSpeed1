using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Realtime;

namespace TickSpeed.TradeScripts
{
    // Скрипт демонстрирует использование интерфейса ISecurityRt для работы с ордерами.
    public class RealtimeOrderTest : IExternalScript
    {
        public static bool Flag { get; set; }

        static RealtimeOrderTest()
        {
            Flag = false;
        }

        public void Execute(IContext ctx, ISecurity sec)
        {
            var rtSec = sec as ISecurityRt;
            const int counter = 20;
            if (rtSec == null)
            {
                ctx.Log("Мы в режиме лаборатории!!!");
                return;
            }
            //var buyFlag = (bool) ctx.LoadGlobalObject("SiM7_Buy");
            //var sellFlag = (bool)ctx.LoadGlobalObject("SiM7_Sell");
            //if (buyFlag)
            //{
            //    var fixsignal = true;
            //    var fixbarnumber = rtSec.Bars.Count;
            //}
            var fixbarnumber = rtSec.Bars.Count;
            var currPos = 0.0;
            Flag = false;
            if (!rtSec.HasActiveOrders)
            {
                if (!Flag)
                {
                    rtSec.NewOrder(OrderType.Limit, true, (double)rtSec.FinInfo.Bid - 10, 1, "LX");
                }
            }
            foreach (var order in rtSec.Orders)
            {
                 if(order.IsActive)
                 {
                    ctx.Log("Номер бара = " + order.FindBarNumber(rtSec.Bars).ToString(), MessageType.Info, true);
                    //rtSec.CancelOrder(order);
                    Flag = true;
                 }

                
                if (order.IsExecuted)
                {
                    currPos += (order.Quantity - order.RestQuantity);
                    ctx.Log("Позиция ==");
                //           rtSec.NewOrder(OrderType.Limit, false, (double)rtSec.FinInfo.Ask + 1, 1, "LX");
                }
            }
            



                // Для исполненных ордеров считаем общую позицию по инструменту. Заявка может быть частично исполнена
                // поэтому учитываем частичное исполнение.
                //if (order.IsExecuted)
                //{
                //    currPos += (order.Quantity - order.RestQuantity);
                //    ctx.LogInfo("Позиция == {0}", currPos);
                //}


                // Скрипт торговли
                // 
                //var currPos = 0D;
                //    foreach (var order in rtSec.Orders)
                //    {
                //        // В
                //        {
                //            ctx.Log("Номер бара ");
                //            //rtSec.CancelOrder(order);
                //        }

                //        // Для исполненных ордеров считаем общую позицию по инструменту. Заявка может быть частично исполнена
                //        // поэтому учитываем частичное исполнение.
                //        if (order.IsExecuted)
                //        {
                //            currPos += (order.Quantity - order.RestQuantity);
                //            ctx.Log("Позиция ==");
                //            rtSec.NewOrder(OrderType.Limit, false, (double)rtSec.FinInfo.Ask + 1, 1, "LX");
                //        }
                //    }
                //if (buyFlag && !rtSec.HasActiveOrders)
                //{
                //    // Выставим новый ордер на покупку.
                //    rtSec.NewOrder(OrderType.Limit, true, (double)rtSec.FinInfo.Bid - 1, 1, "LE");
                //}

                // вывод тиков инструмента на первую панель
            IGraphPane mainPane = ctx.CreateGraphPane("Главная", null);
            mainPane.Visible = true;
            mainPane.HideLegend = false;
            var color1 = new Color(System.Drawing.Color.Green.ToArgb());
            mainPane.AddList("Tick", "tt", sec, CandleStyles.CANDLE_AND_QUEUE, color1, PaneSides.RIGHT);

        }

        public IDictionary<string, object> OrderInfo { get; set; }
    }
}
