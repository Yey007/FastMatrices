
namespace FastMatrixOperations.Samples.GPU
{
    static class GPUAdd
    {
        static void Add()
        {
            //common wrappers are included in this library by default
            //see DefaultTypeOperators.cs under FastMatrixOperations
            GPUOperator<IntWrapper> op = new GPUOperator<IntWrapper>();

            //two 5*3 matrices
            BufferedFastMatrix<IntWrapper> one = new BufferedFastMatrix<IntWrapper>(5, 3);
            BufferedFastMatrix<IntWrapper> two = new BufferedFastMatrix<IntWrapper>(5, 3);

            Utilities.FillMatrix(one, 5);
            Utilities.FillMatrix(two, 10);

            BufferedFastMatrix<IntWrapper> result = op.Add(one, two);
        }
    }
}
