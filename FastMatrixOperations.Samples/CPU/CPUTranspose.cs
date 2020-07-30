using System;

namespace FastMatrixOperations.Samples.CPU
{
    static class CPUTranspose
    { 
        static void Transpose()
        {
            //process is same for parallel
            SingleThreadedOperator<IntWrapper> op = new SingleThreadedOperator<IntWrapper>();

            //5*3 matrix
            FastMatrix<IntWrapper> one = new FastMatrix<IntWrapper>(5, 3);

            Utilities.FillMatrix(one, 5);

            //10 will start at the bottom left and go to the top right
            one[0, one.Rows - 1] = 10;
            FastMatrix<IntWrapper> result = op.Transpose(one);
            Console.WriteLine(result);
        }
    }
}