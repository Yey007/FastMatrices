using Xunit;

namespace FastMatrixOperations.Tests.GPU
{
    public class GPUStructTests : TestBase<Vector3>
    {
        GPUOperator<Vector3, Vector3Operator> gpu = new GPUOperator<Vector3, Vector3Operator>();
        Vector3 twenty = new Vector3(20, 20, 20);
        Vector3 fifteen = new Vector3(15, 15, 15);
        Vector3 five = new Vector3(5, 5, 5);

        [Fact]
        public void Add()
        {
            Vector3[,] expected = MakeResult(size, size, twenty);

            BufferedFastMatrix<Vector3> one = MakeBufferedMatrix(size, size, fifteen);
            BufferedFastMatrix<Vector3> two = MakeBufferedMatrix(size, size, five);
            BufferedFastMatrix<Vector3> actual = gpu.Add(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Subtract()
        {
            Vector3[,] expected = MakeResult(size, size, fifteen);

            BufferedFastMatrix<Vector3> one = MakeBufferedMatrix(size, size, twenty);
            BufferedFastMatrix<Vector3> two = MakeBufferedMatrix(size, size, five);
            BufferedFastMatrix<Vector3> actual = gpu.Subtract(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Multiply()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(375, 375, 375));

            BufferedFastMatrix<Vector3> one = MakeBufferedMatrix(size, size, fifteen);
            BufferedFastMatrix<Vector3> two = MakeBufferedMatrix(size, size, five);
            BufferedFastMatrix<Vector3> actual = gpu.Multiply(one, two);
            VerifyResults(actual, expected);
        }

        [Fact]
        public void Transpose()
        {
            Vector3[,] expected = MakeResult(size, size, fifteen);

            BufferedFastMatrix<Vector3> one = MakeBufferedMatrix(size, size, fifteen);

            one[size - 1, 0] = five;
            expected[0, size - 1] = five;

            BufferedFastMatrix<Vector3> actual = gpu.Transpose(one);
            VerifyResults(actual, expected);
        }
    }
}
