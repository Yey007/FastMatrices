
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUSub
    {
        static void Subtract()
        {
            GPUOperator<IntWrapper> op = new GPUOperator<IntWrapper>();

            //two 5*3 matrices
            BufferedFastMatrix<IntWrapper> one = new BufferedFastMatrix<IntWrapper>(5, 3);
            BufferedFastMatrix<IntWrapper> two = new BufferedFastMatrix<IntWrapper>(5, 3);

            Utilities.FillMatrix(one, 5);
            Utilities.FillMatrix(two, 10);

            BufferedFastMatrix<IntWrapper> result = op.Subtract(one, two);
        }
    }
}