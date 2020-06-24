
namespace FastMatrixOperations.Samples.CPU
{
    static class GPUSub
    {
        static void Subtract()
        {
            //process is same for parallel
            CPUOperator<int> op = new CPUOperator<int>();

            //two 5*3 matrices
            FastMatrix<int> one = new FastMatrix<int>(5, 3);
            FastMatrix<int> two = new FastMatrix<int>(5, 3);

            Utilities.FillMatrix<int>(one, 5);
            Utilities.FillMatrix<int>(two, 10);

            FastMatrix<int> result = op.Subtract(one, two);
        }
    }
}