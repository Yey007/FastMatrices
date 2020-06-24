﻿
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUAdd
    {
        static void Add()
        {
            //common type operators are included in this library by default
            //see DefaultTypeOperators.cs under FastMatrixOperations
            GPUOperator<int, IntOperator> op = new GPUOperator<int, IntOperator>();

            //two 5*3 matrices
            FastMatrix<int> one = new FastMatrix<int>(5, 3);
            FastMatrix<int> two = new FastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);
            Utilities.FillMatrix<int>(two, 10);

            FastMatrix<int> result = op.Add(one, two);
        }
    }
}
