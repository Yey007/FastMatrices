
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUTranspose
    { 
        static void Transpose()
        {
            GPUOperator<IntWrapper> op = new GPUOperator<IntWrapper>();

            //5*3 matrix
            BufferedFastMatrix<IntWrapper> one = new BufferedFastMatrix<IntWrapper>(5, 3);

            Utilities.FillMatrix(one, 5);

            //10 will start at the bottom left and go to the top right
            one[0, one.GetSize(0) - 1] = 10;
            BufferedFastMatrix<IntWrapper> result = op.Transpose(one);
        }
    }
}