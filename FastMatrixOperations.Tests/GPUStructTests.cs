using Xunit;

namespace FastMatrixOperations.Tests
{
    public class GPUStructTests : TestBase<Vector3>
    {
        GPUOperator<Vector3, Vector3Operator> gpu = new GPUOperator<Vector3, Vector3Operator>();

        [Fact]
        public void Add()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(20, 20, 20));

            FastMatrix<Vector3> one = MakeMatrix(size, size, new Vector3(15, 15, 15));
            FastMatrix<Vector3> two = MakeMatrix(size, size, new Vector3(5, 5, 5));
            FastMatrix<Vector3> actual = gpu.Add(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Subtract()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(15, 15, 15));

            FastMatrix<Vector3> one = MakeMatrix(size, size, new Vector3(20, 20, 20));
            FastMatrix<Vector3> two = MakeMatrix(size, size, new Vector3(5, 5, 5));
            FastMatrix<Vector3> actual = gpu.Subtract(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Multiply()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(750, 750, 750));

            FastMatrix<Vector3> one = MakeMatrix(size, size, new Vector3(15, 15, 15));
            FastMatrix<Vector3> two = MakeMatrix(size, size, new Vector3(10, 10, 10));
            FastMatrix<Vector3> actual = gpu.Multiply(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Transpose()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(10, 10, 10));

            FastMatrix<Vector3> one = MakeMatrix(size, size, new Vector3(10, 10, 10));

            one[size - 1, 0] = new Vector3(5, 5, 5);
            expected[0, size - 1] = new Vector3(5, 5, 5);

            FastMatrix<Vector3> actual = gpu.Transpose(one);
            VerifyResults(actual, expected);
        }
    }
}
