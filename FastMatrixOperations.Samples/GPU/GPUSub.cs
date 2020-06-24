﻿
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUSub
    {
        static void Subtract()
        {
            GPUOperator<int, IntOperator> op = new GPUOperator<int, IntOperator>();

            //two 5*3 matrices
            BufferedFastMatrix<int> one = new BufferedFastMatrix<int>(5, 3);
            BufferedFastMatrix<int> two = new BufferedFastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);
            Utilities.FillMatrix<int>(two, 10);

            BufferedFastMatrix<int> result = op.Subtract(one, two);
        }
    }
}