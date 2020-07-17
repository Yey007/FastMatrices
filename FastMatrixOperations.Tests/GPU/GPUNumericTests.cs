using Xunit;

namespace FastMatrixOperations.Tests.GPU
{
    public class GPUNumericTests : TestBase<IntWrapper>
    {
        GPUOperator<IntWrapper> gpu = new GPUOperator<IntWrapper>();

        [Fact]
        public void Add()
        {
            IntWrapper[,] expected = MakeResult(size, size, 20);

            BufferedFastMatrix<IntWrapper> one = MakeBufferedMatrix<IntWrapper>(size, size, 15);
            BufferedFastMatrix<IntWrapper> two = MakeBufferedMatrix<IntWrapper>(size, size, 5);
            BufferedFastMatrix<IntWrapper> actual = gpu.Add(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Subtract()
        {
            IntWrapper[,] expected = MakeResult(size, size, 15);

            BufferedFastMatrix<IntWrapper> one = MakeBufferedMatrix<IntWrapper>(size, size, 20);
            BufferedFastMatrix<IntWrapper> two = MakeBufferedMatrix<IntWrapper>(size, size, 5);
            BufferedFastMatrix<IntWrapper> actual = gpu.Subtract(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Multiply()
        {
            IntWrapper[,] expected = MakeResult(size, size, 750);

            BufferedFastMatrix<IntWrapper> one = MakeBufferedMatrix<IntWrapper>(size, size, 15);
            BufferedFastMatrix<IntWrapper> two = MakeBufferedMatrix<IntWrapper>(size, size, 10);
            BufferedFastMatrix<IntWrapper> actual = gpu.Multiply(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Transpose()
        {
            IntWrapper[,] expected = MakeResult(size, size, 10);

            BufferedFastMatrix<IntWrapper> one = MakeBufferedMatrix<IntWrapper>(size, size, 10);

            one[size - 1, 0] = 5;
            expected[0, size - 1] = 5;

            BufferedFastMatrix<IntWrapper> actual = gpu.Transpose(one);
            VerifyResults(actual, expected);
        }
    }
}
