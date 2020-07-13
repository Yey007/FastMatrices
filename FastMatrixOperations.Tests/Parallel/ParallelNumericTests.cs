using Xunit;

namespace FastMatrixOperations.Tests.Parallel
{
    public class ParallelNumericTests : TestBase<int>
    {
        MultiThreadedOperator<int, IntOperator> cpu = 
            new MultiThreadedOperator<int, IntOperator>();

        [Fact]
        public void Add()
        {
            int[,] expected = MakeResult(size, size, 20);
            UnbufferedFastMatrix<int> matrix = MakeUnbufferedMatrix(size, size, 15);
            UnbufferedFastMatrix<int> matrix2 = MakeUnbufferedMatrix(size, size, 5);
            VerifyResults(cpu.Add(matrix, matrix2), expected);
        }

        [Fact]
        public void Subtract()
        {
            int[,] expected = MakeResult(size, size, 10);
            UnbufferedFastMatrix<int> matrix = MakeUnbufferedMatrix(size, size, 15);
            UnbufferedFastMatrix<int> matrix2 = MakeUnbufferedMatrix(size, size, 5);
            VerifyResults(cpu.Subtract(matrix, matrix2), expected);
        }

        [Fact]
        public void Multiply()
        {
            int[,] expected = MakeResult(size, size, 375);
            UnbufferedFastMatrix<int> matrix = MakeUnbufferedMatrix(size, size, 15);
            UnbufferedFastMatrix<int> matrix2 = MakeUnbufferedMatrix(size, size, 5);
            VerifyResults(cpu.Multiply(matrix, matrix2), expected);
        }

        [Fact]
        public void Transpose()
        {
            int[,] expected = MakeResult(size, size, 10);
            UnbufferedFastMatrix<int> matrix = MakeUnbufferedMatrix(size, size, 10);

            matrix[0, size - 1] = 5;
            expected[size - 1, 0] = 5;

            matrix = cpu.Transpose(matrix);

            VerifyResults(matrix, expected);
        }
    }
}
