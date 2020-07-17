
namespace FastMatrixOperations.Samples.CPU
{
    static class CPUSub
    {
        static void Subtract()
        {
            //process is same for parallel
            SingleThreadedOperator<IntWrapper> op = new SingleThreadedOperator<IntWrapper>();

            //two 5*3 matrices
            FastMatrix<IntWrapper> one = new FastMatrix<IntWrapper>(5, 3);
            FastMatrix<IntWrapper> two = new FastMatrix<IntWrapper>(5, 3);

            Utilities.FillMatrix(one, 5);
            Utilities.FillMatrix(two, 10);

            FastMatrix<IntWrapper> result = op.Subtract(one, two);
            result.Print();
        }
    }
}