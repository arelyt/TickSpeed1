namespace rbftest
{
    class Program
    {
        public static int Main(string[] args)
        {
            double[,] a = new double[,] { { 2, 1, 3 }, { 1, 3, 2 }, { 1, 3, 2 } };
            double[,] b = new double[,] { { 2, 1, 3 }, { 0, 1, 2 }, { 1, 3, 2 } };
            double[,] c = new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            //
            // rmatrixgemm() function allows us to calculate matrix product C:=A*B or
            // to perform more general operation, C:=alpha*op1(A)*op2(B)+beta*C,
            // where A, B, C are rectangular matrices, op(X) can be X or X^T,
            // alpha and beta are scalars.
            //
            // This function:
            // * can apply transposition and/or multiplication by scalar to operands
            // * can use arbitrary part of matrices A/B (given by submatrix offset)
            // * can store result into arbitrary part of C
            // * for performance reasons requires C to be preallocated
            //
            // Parameters of this function are:
            // * M, N, K            -   sizes of op1(A) (which is MxK), op2(B) (which
            //                          is KxN) and C (which is MxN)
            // * Alpha              -   coefficient before A*B
            // * A, IA, JA          -   matrix A and offset of the submatrix
            // * OpTypeA            -   transformation type:
            //                          0 - no transformation
            //                          1 - transposition
            // * B, IB, JB          -   matrix B and offset of the submatrix
            // * OpTypeB            -   transformation type:
            //                          0 - no transformation
            //                          1 - transposition
            // * Beta               -   coefficient before C
            // * C, IC, JC          -   preallocated matrix C and offset of the submatrix
            //
            // Below we perform simple product C:=A*B (alpha=1, beta=0)
            //
            // IMPORTANT: this function works with preallocated C, which must be large
            //            enough to store multiplication result.
            //
            int m = 2;
            int n = 2;
            int k = 2;
            double alpha = 1.0;
            int ia = 0;
            int ja = 0;
            int optypea = 0;
            int ib = 1;
            int jb = 1;
            int optypeb = 0;
            double beta = 0.0;
            int ic = 0;
            int jc = 0;
            alglib.rmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, ref c, ic, jc);
            System.Console.WriteLine("{0}", alglib.ap.format(c, 3)); // EXPECTED: [[4,3],[2,4]]

            //
            // Now we try to apply some simple transformation to operands: C:=A*B^T
            //
            optypeb = 1;
            alglib.rmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, ref c, ic, jc);
            System.Console.WriteLine("{0}", alglib.ap.format(c, 3)); // EXPECTED: [[5,1],[5,3]]
            System.Console.ReadLine();
            return 0;
        }
    }
}
