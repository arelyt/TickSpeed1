namespace rbftest
{
    class Program
    {
        static void Main()
        {

            //
            // This example illustrates basic concepts of the RBF models: creation, modification,
            // evaluation.
            // 
            // Suppose that we have set of 2-dimensional points with associated
            // scalar function values, and we want to build a RBF model using
            // our data.
            // 
            // NOTE: we can work with 3D models too :)
            // 
            // Typical sequence of steps is given below:
            // 1. we create RBF model object
            // 2. we attach our dataset to the RBF model and tune algorithm settings
            // 3. we rebuild RBF model using QNN algorithm on new data
            // 4. we use RBF model (evaluate, serialize, etc.)
            //

            //
            // Step 1: RBF model creation.
            //
            // We have to specify dimensionality of the space (2 or 3) and
            // dimensionality of the function (scalar or vector).
            //
            // New model is empty - it can be evaluated,
            // but we just get zero value at any point.
            //
            alglib.rbfmodel model;
            alglib.rbfcreate(2, 1, out model);

            var v = alglib.rbfcalc2(model, 0.0, 0.0);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 0.000

            //
            // Step 2: we add dataset.
            //
            // XY contains two points - x0=(-1,0) and x1=(+1,0) -
            // and two function values f(x0)=2, f(x1)=3.
            //
            // We added points, but model was not rebuild yet.
            // If we call rbfcalc2(), we still will get 0.0 as result.
            //
            double[,] xy = new double[,] { { -1, 0, 2 }, { +1, 0, 3 } };
            alglib.rbfsetpoints(model, xy);
            System.Console.WriteLine("{0:F2}", xy);
            System.Console.ReadLine();

            v = alglib.rbfcalc2(model, 0.0, 0.0);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 0.000

            //
            // Step 3: rebuild model
            //
            // After we've configured model, we should rebuild it -
            // it will change coefficients stored internally in the
            // rbfmodel structure.
            //
            // We use hierarchical RBF algorithm with following parameters:
            // * RBase - set to 1.0
            // * NLayers - three layers are used (although such simple problem
            //   does not need more than 1 layer)
            // * LambdaReg - is set to zero value, no smoothing is required
            //
            alglib.rbfreport rep;
            alglib.rbfsetalgomultilayer(model, 1.0, 3, 0.0);
            alglib.rbfbuildmodel(model, out rep);
            System.Console.WriteLine("{0}", rep.terminationtype); // EXPECTED: 1

            //
            // Step 4: model was built
            //
            // After call of rbfbuildmodel(), rbfcalc2() will return
            // value of the new model.
            //
            v = alglib.rbfcalc2(model, 0.0, 0.0);
            System.Console.WriteLine("{0:F2}", v); // EXPECTED: 2.500
            System.Console.ReadLine();
        }
    }
}
