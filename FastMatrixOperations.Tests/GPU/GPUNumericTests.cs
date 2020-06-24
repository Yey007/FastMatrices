using Xunit;

namespace FastMatrixOperations.Tests.GPU
{
    public class GPUNumericTests : TestBase<int>
    {
        GPUOperator<int, IntOperator> gpu = new GPUOperator<int, IntOperator>();

        [Fact]
        public void Add()
        {
            int[,] expected = MakeResult(size, size, 20);

            BufferedFastMatrix<int> one = MakeBufferedMatrix(size, size, 15);
            BufferedFastMatrix<int> two = MakeBufferedMatrix(size, size, 5);
            BufferedFastMatrix<int> actual = gpu.Add(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Subtract()
        {
            int[,] expected = MakeResult(size, size, 15);

            BufferedFastMatrix<int> one = MakeBufferedMatrix(size, size, 20);
            BufferedFastMatrix<int> two = MakeBufferedMatrix(size, size, 5);
            BufferedFastMatrix<int> actual = gpu.Subtract(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Multiply()
        {
            int[,] expected = MakeResult(size, size, 750);

            BufferedFastMatrix<int> one = MakeBufferedMatrix(size, size, 15);
            BufferedFastMatrix<int> two = MakeBufferedMatrix(size, size, 10);
            BufferedFastMatrix<int> actual = gpu.Multiply(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Transpose()
        {
            int[,] expected = MakeResult(size, size, 10);

            BufferedFastMatrix<int> one = MakeBufferedMatrix(size, size, 10);

            one[size - 1, 0] = 5;
            expected[0, size - 1] = 5;

            BufferedFastMatrix<int> actual = gpu.Transpose(one);
            VerifyResults(actual, expected);
        }
    }
}
