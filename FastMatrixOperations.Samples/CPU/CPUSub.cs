
namespace FastMatrixOperations.Samples.CPU
{
    static class CPUSub
    {
        static void Subtract()
        {
            //process is same for parallel
            SingleThreadedOperator<int, IntOperator> op =
                new SingleThreadedOperator<int, IntOperator>();

            //two 5*3 matrices
            UnbufferedFastMatrix<int> one = new UnbufferedFastMatrix<int>(5, 3);
            UnbufferedFastMatrix<int> two = new UnbufferedFastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);
            Utilities.FillMatrix<int>(two, 10);

            UnbufferedFastMatrix<int> result = op.Subtract(one, two);
        }
    }
}