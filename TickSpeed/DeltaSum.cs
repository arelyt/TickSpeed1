using System;
using System.Collections.Generic;
using System.Linq;
using TSLab.DataSource;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Скорость сделок на покупку/продажу в секунду.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("DeltaSum")]
#pragma warning restore 612
    public class DeltaSumClass : IBar2DoubleHandler
    {
        [HandlerParameter(Name = "Объем_тики", Default = "true", NotOptimized = true)]
        public bool Method { get; set; }

        [HandlerParameter(Name = "Дробь_дельта", Default = "true", NotOptimized = true)]
        public bool Delta { get; set; }

        [HandlerParameter(Name = "minusplus", Default = "false", NotOptimized = true)]
        public bool Sign { get; set; }

        [HandlerParameter(Name = "Шаг дельты", Default = "50", Min = "2", Max = "2000", Step = "1")]
        public int Step { get; set; }

        public IList<double> Execute(ISecurity security)
        {
            var count = security.Bars.Count;
            //var values = new double[count];
            if (count < 2)
                return null;

            var _cumdelta = 0;
            //values[0] = 1;
            var _indi = 0;
            var freqB = new int[count];
            var freqS = new int[count];
            var sumBS = new double[count];

            for (var i = 0; i < count; i++)
            {
                var trades = security.GetTrades(i);
                if (Method)
                {
                    freqB[i] = (int) trades.Sum(t => t.Direction == TradeDirection.Buy ? t.Quantity : 0);
                    ;
                    freqS[i] = (int) trades.Sum(t => t.Direction == TradeDirection.Sell ? t.Quantity : 0);
                    ;
                }
                else
                {
                    freqB[i] = trades.Sum(t => t.Direction == TradeDirection.Buy ? 1 : 0);
                    ;
                    freqS[i] = trades.Sum(t => t.Direction == TradeDirection.Sell ? 1 : 0);
                    ;
                }


            }

            for (int i = 0; i < count; i++)
            {
                int sec = 1;
                int sumB = 0;
                int sumS = 0;

                if (Sign)
                {
                    while (sumB - sumS < Step && i > 10)
                    {
                        sumB = DeltaSumClass.Summ(sec, i, freqB);
                        sumS = DeltaSumClass.Summ(sec, i, freqS);
                        sec++;
                    }
                }
                else
                {
                    while (sumB + sumS < Step && i > 10)
                    {
                        sumB = DeltaSumClass.Summ(sec, i, freqB);
                        sumS = DeltaSumClass.Summ(sec, i, freqS);
                        sec++;
                    }
                }
                

                if (Delta)
                {
                    if (sumS + sumB != 0) sumBS[i] = ((double) sumB - (double) sumS) / ((double) sumB + (double) sumS);
                    else sumBS[i] = 0.0;
                }
                else
                {
                    if (sumS != 0) sumBS[i] = (double) sumB / (double) sumS;
                    else sumBS[i] = 1;
                }
            }

            return sumBS.ToList();
        }


        private static int Summ(int intsec, int N, int[] freq)
        {
            int sum = 0;
            var spr = N - intsec < 0 ? N : intsec;
            for (int i = N - spr; i <= N; ++i)
            {
                sum += freq[i];
            }

            return sum;

        }
    }
}
