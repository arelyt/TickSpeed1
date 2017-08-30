﻿using System;
using System.Collections.Generic;
using System.Globalization;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
// ReSharper disable InconsistentNaming

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSA")]
#pragma warning restore 612
    public class SsaClass : IOneSourceHandler, IDoubleInputs, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface ISsaFor
        {
            
            double[] ssa1(double[] in1, int in2, int in3);

        }

        [HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        public int Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numrec { get; set; }

        //[HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numfor { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Signal denoising process

            // Wavelet DB3 Level 5
            var t = DateTime.Now;
            MWClient client = new MWHttpClient();
            try
            {
                ISsaFor sigDen = client.CreateProxy<ISsaFor>(new Uri("http://localhost:9910/ssa1_dep"));
                result = sigDen.ssa1(values, Numdec, Numrec);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssa exec for "+g+" msec", MessageType.Info, toMessageWindow:true);
            return result;
        }

        

        
    }
}