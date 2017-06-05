﻿using System;
using System.Collections.Generic;
using TSLab.Script.Handlers;
using MathWorks.MATLAB.ProductionServer.Client;

namespace TickSpeed
{
    // Объемно-тиковый осциллятор.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("Envelope")]
#pragma warning restore 612
    public class EnvelopeClass : IDouble2DoubleHandler, IValuesHandlerWithNumber
    {
        public interface IEnvelope
        {
            double[] envel_auto(double[] in1, double in2, string in3);
        }

        [HandlerParameter(Name = "Method", NotOptimized = true)]
        public V2.Method Method { get; set; }

        [HandlerParameter(Name = "Window", Default = "128")]
        public double Win { get; set; }

        public IList<double> Execute(IList<double> myDoubles)
        {
            string name;
            switch (Method)
            {
                case V2.Method.Analytic:
                    name = "analytic";
                    break;
                case V2.Method.Rms:
                    name = "rms";
                    break;
                case V2.Method.Peak:
                    name = "peak";
                    break;
                default:
                    name = "peak";
                    break;
            }
            var count = myDoubles.Count;
            var result = new double[count];
            var values = new double[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = myDoubles[i];
            }
            // Начинаем Envelope process

            
            MWClient client = new MWHttpClient();
            try
            {
                IEnvelope sigDen = client.CreateProxy<IEnvelope>(new Uri("http://localhost:9910/envel_auto_dep"));
                result = sigDen.envel_auto(values, Win, name);
            }
            catch (MATLABException)
            {

            }
            finally
            {
                client.Dispose();
            }
            return result;
        }
    }
}