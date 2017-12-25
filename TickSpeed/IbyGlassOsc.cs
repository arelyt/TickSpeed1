using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using MoreLinq;
using RusAlgo.Helper;
//using System.Linq;
//using MoreLinq;
//using RusAlgo.Helper;
using TSLab.Script;
using TSLab.Script.Handlers;

namespace TickSpeed
{

    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("IbyGlassOsc")]
#pragma warning restore 612
    public class IbyGlassOscClass : ISecurityInputs, IDoubleReturns, IOneSourceHandler, IStreamHandler, IContextUses
    {
        [HandlerParameter(true, "0", Min = "1", Max = "20", Step = "1")]
        public int Level { get; set; }
        [HandlerParameter(Name = "Reverse", Default = "true", NotOptimized = false)]
        public bool Reverse { get; set; }
        [HandlerParameter(true, "glass", Name = "ObjName", NotOptimized = false)]
        public string Objname { get; set; }
        public IContext Context { get; set; }

        //public IList<double> glass { get; set; }
        //private static IList<double> Glass { get; set; }
        public IList<double> Execute(ISecurity source)
        {
            var ctx = Context;
            var count = source.Bars.Count;
            
            var maxlev = source.GetSellQueue(0).Count;
            var numArray = 0.0;
            if (count < 10 || maxlev == 0)
                return null;
            var temp = (List<double>)ctx.LoadGlobalObject(Objname);
            if (temp.IsNull())
            {
                var glass = new double[count];

                for (int i = 0; i < count; i++)
                {
                    glass[i] = 0.0;
                }
                var rew = glass.ToList();
                ctx.StoreGlobalObject(Objname, rew);
                temp = (List<double>)ctx.LoadGlobalObject(Objname);
            }
            //if (TradeHelper.IsNull(Glass) || Reset)
            //{
            //    var numarray = new double[count];
            //    Glass = numarray.ToList();
            //    var nSell = 0.0;
            //    var nBuy = 0.0;
            //    var sellQueue = source.GetSellQueue(0);
            //    var buyQueue = source.GetBuyQueue(0);
            //    for (var i = 0; i < maxlev; i++)
            //    {
            //        nSell += sellQueue[i].Quantity;
            //        nBuy += buyQueue[i].Quantity;
            //    }
            //    Glass[count] = nBuy/(nBuy + nSell) -1;

            //}
            //else
            //{
            //    var nSell = 0.0;
            //    var nBuy = 0.0;
            //    var sellQueue = source.GetSellQueue(0);
            //    var buyQueue = source.GetBuyQueue(0);
            //    for (var i = 0; i < maxlev; i++)
            //    {
            //        nSell += sellQueue[i].Quantity;
            //        nBuy += buyQueue[i].Quantity;
            //    }
            //    Glass.Add(nBuy / (nBuy + nSell) - 1);
            //}

            //return Glass.TakeLast(count).ToArray();
            //for (int i = 0; i < count; i++)
            //{
            var nSell = 0.0;
            var nBuy = 0.0;
            var sellQueue = source.GetSellQueue(0);
            var buyQueue = source.GetBuyQueue(0);
            for (var j = 0; j < Level; j++)
            {
                nSell += sellQueue[j].Quantity;
                nBuy += buyQueue[j].Quantity;
            }
            if (Reverse)
            {
                numArray = (nBuy - nSell) / (nBuy + nSell);
            }
            else
            {
                numArray = (nSell - nBuy) / (nBuy + nSell);
            }
                
            //}
            
            temp.Add(numArray);
            
            ctx.StoreGlobalObject(Objname, temp);
            return temp;
        }
        
    }
}

