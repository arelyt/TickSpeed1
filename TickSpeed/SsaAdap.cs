﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;
// ReSharper disable InconsistentNaming

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SSAAdap")]
#pragma warning restore 612
    public class SsaAdapClass : ITwoSourcesHandler, IDoubleInput0, IDoubleInput1, IDoubleReturns, IStreamHandler, IContextUses
    {
        public IContext Context { get; set; }
        public interface ISsaFor
        {
            
            double[] ssarec(double[] in1, int in2, int in3);

        }

        //[HandlerParameter(true, "60", Name = "Win", Max = "1000", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numdec { get; set; }

        [HandlerParameter(true, "5", Name = "NumCompRec", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        public int Numrec { get; set; }

        //[HandlerParameter(true, "1", Name = "NumForForecast", Max = "10", Min = "1", Step = "1", NotOptimized = false)]
        //public int Numfor { get; set; }

        public IList<double> Execute(IList<double> myDoubles, IList<double> sp)
        {
            var count = myDoubles.Count;
            var speed = (int) Math.Floor(sp.Last() * 10);
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
                ISsaFor sigDen = client.CreateProxy<ISsaFor>(new Uri("http://localhost:9910/ssarec_dep"));
                result = sigDen.ssarec(values, speed, Numrec);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            var g = (DateTime.Now - t).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            Context.Log("ssaadap exec for "+g+" msec", MessageType.Info, toMessageWindow:true);
            return result;
        }

        

        
    }
}