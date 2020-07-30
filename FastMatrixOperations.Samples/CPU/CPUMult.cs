using System;

namespace FastMatrixOperations.Samples.CPU
{
    static class CPUMult
    {
        static void Multiply()
        {
            //process is same for parallel
            SingleThreadedOperator<IntWrapper> op = new SingleThreadedOperator<IntWrapper>();

            //two 5*3 matrices
            FastMatrix<IntWrapper> one = new FastMatrix<IntWrapper>(5, 3);
            FastMatrix<IntWrapper> two = new FastMatrix<IntWrapper>(3, 5);

            Utilities.FillMatrix(one, 5);
            Utilities.FillMatrix(two, 10);

            FastMatrix<IntWrapper> result = op.Multiply(one, two);
            Console.WriteLine(result);
        }
    }
}