using Xunit;

namespace FastMatrixOperations.Tests
{
    public class GPUNumericTests : TestBase<int>
    {
        GPUOperator<int, IntOperator> gpu = new GPUOperator<int, IntOperator>();

        [Fact]
        public void Add()
        {
            int[,] expected = MakeResult(size, size, 20);

            FastMatrix<int> one = MakeMatrix(size, size, 15);
            FastMatrix<int> two = MakeMatrix(size, size, 5);
            FastMatrix<int> actual = gpu.Add(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Subtract()
        {
            int[,] expected = MakeResult(size, size, 15);

            FastMatrix<int> one = MakeMatrix(size, size, 20);
            FastMatrix<int> two = MakeMatrix(size, size, 5);
            FastMatrix<int> actual = gpu.Subtract(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Multiply()
        {
            int[,] expected = MakeResult(size, size, 750);

            FastMatrix<int> one = MakeMatrix(size, size, 15);
            FastMatrix<int> two = MakeMatrix(size, size, 10);
            FastMatrix<int> actual = gpu.Multiply(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Transpose()
        {
            int[,] expected = MakeResult(size, size, 10);

            FastMatrix<int> one = MakeMatrix(size, size, 10);

            one[size - 1, 0] = 5;
            expected[0, size - 1] = 5;

            FastMatrix<int> actual = gpu.Transpose(one);
            VerifyResults(actual, expected);
        }
    }
}
