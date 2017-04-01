﻿using System.Collections.Generic;
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

    public class Ts2Class
    {
        //private double nB, nS, vB, vS;
        public static IList<Ts2> Execute(ISecurity sec)
        {
            var count = sec.Bars.Count;
            var values = new Ts2[count];



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
    
    }