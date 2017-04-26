/*================================================================================
 * Стратегия: First Wavelet Strategy based on 4 Tick decompodsition
 * Платформа: TSLab версия 2.0.16.0
 * Дата создания: 26.04.2017
 * Реализовано: Arelyt
 *================================================================================*/
using System;
using System.Collections.Generic;
using TSLab.Script;
using TSLab.Script.Handlers;
using TSLab.Script.Optimization;

namespace TickSpeed.V2
{
    //================================================================================

    public class FirstTickGlobal : IExternalScript
    {
        // используем переменные-флаги для сигналов
        public bool bBuy; // флаг сигнала открытия длинной позиции
        public bool bSell;
        public bool bShort;
        public bool bCover;
        public IPosition LongPos, ShortPos;

        public virtual void Execute(IContext ctx, ISecurity sec)
        {

            if (sec.IntervalBase.ToString() != "TICK" || sec.Interval.ToString() != "1")
                throw new Exception("Base Interval wrong. Please set to Tick 1");

            //var cache = ctx.LoadGlobalObject("TickPrice");

            #region основной цикл - проход по барам
            int barsCount = source.Bars.Count;
            for (int bar = StartBar; bar < barsCount; bar++)
            {
                //--------------------------------------------------------------------------------
                #region calculate values
                //Price = source.ClosePrices[bar];
                vRSI = nRSI[bar];
                vRSI1 = nRSI[bar - 1];
                vRSI2 = nRSI[bar - 2];
                vMACD = nMACD[bar];
                vMAS = nMAS[bar];


                #endregion
                //--------------------------------------------------------------------------------
                #region data series


                #endregion
                //--------------------------------------------------------------------------------
                #region generate signals
                // сброс значений сигналов
                bBuy = false;
                bSell = false;
                bShort = false;
                bCover = false;

                // установка сигналов по условиям
                if (vMACD > vMAS && vRSI1 < 30 && vRSI1 < vRSI && vRSI1 < vRSI2)
                {
                    bBuy = true;
                }
                if (vMACD < vMAS && vRSI1 > 70 && vRSI1 > vRSI && vRSI1 > vRSI2)
                {
                    bShort = true;
                }
                if (vRSI > 70) bSell = true;
                if (vRSI < 30) bCover = true;

                //bBuy = true;
                //bCover = true;
                //bShort = true;
                //bSell = true;

                #endregion
                //================================================================================
                #region execute signals
                //--------------------------------------------------------------------------------
                // выполнение сигналов для длинной позиции
                IPosition LongPos = source.Positions.GetLastActiveForSignal("LN");
                if (LongPos == null)
                {
                    // Если нет активной длинной позиции
                    if (bBuy)
                    {
                        // Если есть сигнал Buy, 
                        // выдаем ордер на открыте новой длинной позиции.
                        source.Positions.BuyAtMarket(bar + 1, 1, "LN");
                    }
                }
                else
                {
                    // Если есть активная длинная позиция 
                    if (bSell)
                    {
                        // Если есть сигнал Sell, 
                        // выдаем ордер на закрыте длинной позиции.
                        LongPos.CloseAtMarket(bar + 1, "LX");
                    }
                }
                //--------------------------------------------------------------------------------
                // выполнение сигналов для короткой позиции
                IPosition ShortPos = source.Positions.GetLastActiveForSignal("SN");
                if (ShortPos == null)
                {
                    // Если нет активной короткой позиции
                    if (bShort)
                    {
                        // Если есть сигнал Short
                        // выдаем ордер на открыте новой короткой позиции.
                        source.Positions.SellAtMarket(bar + 1, 1, "SN");

                    }
                }
                else
                {
                    // Если есть активная короткая позиция, 
                    if (bCover)
                    {
                        // Если есть сигнал Cover
                        // выдаем ордер на закрыте короткой позиции.
                        ShortPos.CloseAtMarket(bar + 1, "SX");
                    }
                }

                #endregion
            }
            #endregion

            var price = new double[ctx.BarsCount];
            for (var i = 0; i < ctx.BarsCount; i++)
            {
               //var t = sec.GetTrades(i);
                price[i] = sec.GetTrades(i)[0].Price;
            }
            
            ctx.StoreGlobalObject("TickPrice", price);    

        }
    }
}