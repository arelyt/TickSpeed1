using System.Collections.Generic;
using System.Linq;
using TSLab.Script;
using TSLab.Script.Handlers;
using static alglib;

namespace TickSpeed
{
    // RBF model from AlgoLib.
    [HandlerCategory("Arelyt")]
#pragma warning disable 612
    [HandlerName("SsaAlgLib")]
#pragma warning restore 612
    public class SsaAlgLibClass : IDouble2DoubleHandler
    {
        [HandlerParameter(Name = "Win", Default = "60", NotOptimized = false)]
        public int L { get; set; }
        [HandlerParameter(Name = "Nrec", Default = "0.01", NotOptimized = false)]
        public int N { get; set; }
        
        public IList<double> Execute(IList<double> signal)
        {
            var T = signal.Count;
            var values = signal.ToList();
            V2.Interpolation.InterpolateNan(ref values);
            var K = T - L + 1;
            if (T < 10)
                return null;
            
            var result = new double[T];
            
            var X = new double[L, K];
            // Build trajectory matrix
            for (var i = 0; i < K-1; i++)
            {
                for (var j = 0; j < L; j++)
                {
                    X[j, i] = values[i+j];
                }
                
            }
            // S=X*X^T
            int m = L;
            int n = L;
            int k = K;
            double alpha = 1.0;
            int ia = 0;
            int ja = 0;
            int optypea = 0;
            int ib = 0;
            int jb = 0;
            int optypeb = 1;
            double beta = 0.0;
            int ic = 0;
            int jc = 0;
            alglib.setnworkers(0);
            var S = new double[L, L];
            alglib.smp_rmatrixgemm(m, n, k, alpha, X, ia, ja,
                optypea, X, ib, jb, optypeb, beta, ref S, ic, jc);
            int uneeded = 0;
            int vtneeded = 2;
            int additionalmemory = 2;
            double[] G;
            double[,] U;
            double[,] R;
            var svdres = alglib.smp_rmatrixsvd(S, m, n, uneeded, vtneeded,
                additionalmemory, out G, out U, out R);
            // ttttt
            optypea = 1;
            optypeb = 0;
            m = S.GetLength(0);
            n = U.GetLength(1);
            k = S.GetLength(1);
            var V = new double[m, n];
            alglib.smp_rmatrixgemm(m, n, k, alpha, X, ia, ja,
                optypea, R, ib, jb, optypeb, beta, ref V, ic, jc);
            // Grouping
            var In = new int[N];
            for (int i = 0; i < N; i++)
            {
                In[i] = i;
            }
            optypea = 0;
            optypeb = 1;
            m = U.GetLength(0);
            n = V.GetLength(1);
            k = U.GetLength(1);
            var rca = new double[m, n];
            alglib.smp_rmatrixgemm(m, n, k, alpha, U, ia, ja,
                optypea, V, ib, jb, optypeb, beta, ref rca, ic, jc);


            for (var i = 0; i < count; i++)
            {
                result[i] = rbfcalc2(_model, time[i], 0.0);
            }
            return result;
        }
    }
}