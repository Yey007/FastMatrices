
namespace FastMatrixOperations.Samples.CPU
{
    static class CPUTranspose
    { 
        static void Transpose()
        {
            //process is same for parallel
            SingleThreadedOperator<int> op = new SingleThreadedOperator<int>();

            //5*3 matrix
            UnbufferedFastMatrix<int> one = new UnbufferedFastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);

            //10 will start at the bottom left and go to the top right
            one[0, one.GetSize(0) - 1] = 10;
            UnbufferedFastMatrix<int> result = op.Transpose(one);
        }
    }
}