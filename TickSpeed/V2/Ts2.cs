using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using TSLab.Script;

namespace TickSpeed.V2
{
    // Инициализируем структуру, которая будет хранить все значения
    // осциллятора на всех уровнях декомпозиции от 4 до 512

    public struct Ts2
    {
        public double vto4;
        public double vto8;
        public double vto16;
        public double vto32;
        public double vto64;
        public double vto128;
        public double vto256;
        public double vto512;

        //public int Sp(Step)
        //{
            
        //    int Val;
        //    switch (Step)
        //    {
        //        case Step.S4:
        //            Val = 4;
        //            break;
        //    }
        //    return Val;
        //}

    }
    public enum Wavelets
    {
        Daubechies = 0,
        Symlets = 1

    }

    public enum ThreshRule
    {
        Rigrsure = 0,
        Heursure = 1,
        Sqtwolog = 2,
        Minimaxi = 3,
        Modwtsqtwolog = 4
    }

    public enum Method
    {
        Rms = 0,
        Analytic = 1,
        Peak = 2
    }

    public enum Boundary
    {
        Upper = 0,
        Lower = 1
    }

    public enum Predin
    {
        Upper = 0,
        Values = 1,
        Lower = 2
    }

    public enum Scal
    {
        One = 0,
        Sln = 1,
        Mln = 2
    }

    public enum Step
    {
        S4 = 0,
        S8 = 1,
        S16 = 2,
        S32 = 3,
        S64 = 4,
        S128 = 5,
        S256 = 6,
        S512 = 7

    }

    public enum Extmode
    {
        Zpd = 0,
        Sp0 = 1,
        Sp1 = 2,
        Sym = 3,
        Symw = 4,
        Asym = 5,
        Asymw = 6,
        Ppd = 7,
        Per = 8
    }

    public enum RbfFunction
    {
        Gaussian = 0,
        Thinplate = 1,
        Cubic = 2,
        Multicuadrics = 3,
        Linear = 4
    }

    public enum RbfAlgLibMethodOfInput
    {
        Close = 0,
        Bid = 1,
        Ask = 2,
        HalfBidAsk =3
    }

    public enum CumDeltaType
    {
        Tick = 0,
        Volume = 1,
        Price = 2,
        PriceBuy = 3,
        PriceSell = 4
    }

    public class Ts2Class
    {
        //private double nB, nS, vB, vS;
        public static IList<Ts2> Execute(ISecurity sec, int in1)
        {
            var count = sec.Bars.Count;
            var values = new Ts2[count];



            return values;
        }

        //Метод для слайсинга ОТО с шагом Step
        public double[] vto_step(ISecurity scr, int in1)
        {
            var count = Convert.ToInt32(scr.Bars.Count / in1);
            var values = new double[count];
            for (var i = 0; i < count-1; i+=in1)
            {
                double valueTickBuy = 0, valueTickSell = 0, valueVolBuy = 0, valueVolSell = 0;
                var t = scr.GetTradesPerBar(i, i + in1 - 1);
                foreach(var trades in t)
                {
                    valueTickBuy += trades[0].Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += trades[0].Direction.ToString() == "Buy" ? trades[0].Quantity : 0;
                    valueTickSell += trades[0].Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += trades[0].Direction.ToString() == "Sell" ? trades[0].Quantity : 0;

                }
                values[i] = (valueTickBuy - valueTickSell) / (valueTickBuy + valueTickSell) *
                            (valueVolBuy - valueVolSell) / (valueVolBuy + valueVolSell);
            }
            return values;
        }

        public double[] vto_m(ISecurity sec, int in1)
        {
            var values = new double[in1];
            values[0] = 0;

            for (var i = 0; i < sec.Bars.Count; i += in1)
            {
                double valueTickBuy = 0, valueTickSell = 0, valueVolBuy = 0, valueVolSell = 0;
                for (var j = i; j < i + in1; j++)
                {
                    var t = sec.GetTrades(j);
                    valueTickBuy += t[0].Direction.ToString() == "Buy" ? 1 : 0;
                    valueVolBuy += t[0].Direction.ToString() == "Buy" ? t[0].Quantity : 0;
                    valueTickSell += t[0].Direction.ToString() == "Sell" ? 1 : 0;
                    valueVolSell += t[0].Direction.ToString() == "Sell" ? t[0].Quantity : 0;
                    
                }
                for (int k = i*in1+1; k < i+in1-2; k++)
                {
                    values[k] = values[k-1];
                }
                values[i + in1 - 1] = ((valueTickBuy * valueVolBuy - valueTickSell * valueVolSell) /
                                     (valueTickBuy * valueVolBuy + valueTickSell * valueVolSell));
            }
            return values;
        }
    }
    public static class Interpolation
    {
        public static void InterpolateNan(ref List<double> list)
        {
            if (double.IsNaN(list[0]))
            {
                list[0] = list.First(t => !double.IsNaN(t));
            }

            if (double.IsNaN(list[list.Count - 1]))
            {
                list[list.Count - 1] = list.Last(t => !double.IsNaN(t));
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!double.IsNaN(list[i]))
                {
                    continue;
                }

                int begin = 0;
                int end = 0;

                for (int j = i; j >= 0; j--)
                {
                    begin = j;

                    if (!double.IsNaN(list[j]))
                    {
                        break;
                    }
                }

                for (int j = i; j < list.Count; j++)
                {
                    end = j;

                    if (!double.IsNaN(list[j]))
                    {
                        i = j;
                        break;
                    }
                }

                double delta = (list[end] - list[begin]) / (end - begin);
                double newVal = list[begin];

                for (int j = begin + 1; j < end; j++)
                {
                    newVal += delta;
                    list[j] = newVal;
                }
            }
        }
    }    
}
