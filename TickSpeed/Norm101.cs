using System.Collections.Generic;
using TSLab.DataSource;
using TSLab.Script.Handlers;
using TSLab.Script.Helpers;

namespace TickSpeed
{
#pragma warning disable 612
    [HandlerName("Norma101 [-1.0.1]")]
#pragma warning restore 612
    [HandlerCategory("Arelyt")]
    public class Normalizer101 : IContextUses, IDouble2DoubleHandler
    {
        public IList<double> Execute(IList<double> src)
        {
            return this.Context.GetData("normalized101", new string[2]
            {
                this.NormalizationPeriod.ToString(),
                src.GetHashCode().ToString()
            }, (CacheObjectMaker<IList<double>>)(() => Normalizer101.GenNormalizer01(src, this.NormalizationPeriod, this.Context)));
        }

        public static IList<double> GenNormalizer01(IList<double> _price, int _NormPeriod, IContext _ctx)
        {
            int count = _price.Count;
            double[] numArray = new double[count];
            IList<double> data1 = _ctx.GetData("llv", new string[2]
            {
                _NormPeriod.ToString(),
                _price.GetHashCode().ToString()
            }, (CacheObjectMaker<IList<double>>)(() => Series.Lowest(_price, _NormPeriod)));
            IList<double> data2 = _ctx.GetData("hhv", new string[2]
            {
                _NormPeriod.ToString(),
                _price.GetHashCode().ToString()
            }, (CacheObjectMaker<IList<double>>)(() => Series.Highest(_price, _NormPeriod)));
            for (int index = 0; index < count; ++index)
            {
                double num1 = data1[index];
                double num2 = data2[index];
                numArray[index] = num1 == num2 ? 0.0 : 2.0 * (_price[index] - num1) / (num2 - num1) - 1.0;
            }
            return (IList<double>)numArray;
        }

        public IContext Context { get; set; }

        [HandlerParameter(true, "1000", Max = "10000", Min = "1", Step = "1")]
        public int NormalizationPeriod { get; set; }
    }
}