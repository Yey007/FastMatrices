
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUTranspose
    { 
        static void Transpose()
        {
            GPUOperator<int, IntOperator> op = new GPUOperator<int, IntOperator>();

            //5*3 matrix
            FastMatrix<int> one = new FastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);

            //10 will start at the bottom left and go to the top right
            one[0, one.GetSize(0) - 1] = 10;
            FastMatrix<int> result = op.Transpose(one);
        }
    }
}