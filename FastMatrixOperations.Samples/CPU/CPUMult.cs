
namespace FastMatrixOperations.Samples.CPU
{
    static class CPUMult
    {
        static void Multiply()
        {
            //process is same for parallel
            SingleThreadedOperator<int, IntOperator> op =
                new SingleThreadedOperator<int, IntOperator>();

            //two 5*3 matrices
            UnbufferedFastMatrix<int> one = new UnbufferedFastMatrix<int>(5, 3);
            UnbufferedFastMatrix<int> two = new UnbufferedFastMatrix<int>(3, 5);

            Utilities.FillMatrix<int>(one, 5);
            Utilities.FillMatrix<int>(two, 10);

            UnbufferedFastMatrix<int> result = op.Multiply(one, two);
        }
    }
}