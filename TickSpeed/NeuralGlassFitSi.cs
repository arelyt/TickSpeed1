﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.ProductionServer.Client;
using MoreLinq;
using TSLab.Script.Handlers;

namespace TickSpeed
{
    // Скользящее нормализованное окно накопительной дельты
#pragma warning disable 612
    [HandlerCategory("Arelyt")]
    [HandlerName("NeuralFitSi")]
#pragma warning restore 612
    public class NeuralFitSiClass : IDouble2DoubleHandler
    {
        public interface INeuralFitSi
        {
            // ReSharper disable once InconsistentNaming
            double[] mynet1(double[] in1, double in2);
        }
        [HandlerParameter(true, "10.0", Name = "NN")]
        public double Nn { get; set; }

        [HandlerParameter(true, "300", Name = "Win")]
        public int Win { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            
            var count = myDoubles.Count;
            if (count < 2)
                return null;
            var values = new double[Win]; // values result
            var result = new double[count];
            MWClient client = new MWHttpClient();
            try
            {
                INeuralFitSi sigDen = client.CreateProxy<INeuralFitSi>(new Uri("http://localhost:9910/mynet1_dep"));
                values = sigDen.mynet1(myDoubles.TakeLast(Win).ToArray(), Nn);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            values.CopyTo(result, count-Win);
            return result;
        }
    }
}