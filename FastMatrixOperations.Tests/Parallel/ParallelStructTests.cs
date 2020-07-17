using Xunit;

namespace FastMatrixOperations.Tests.Parallel
{
    public class ParallelStructTests : TestBase<Vector3>
    {
        MultiThreadedOperator<Vector3> cpu = new MultiThreadedOperator<Vector3>();

        Vector3 twenty = new Vector3(20, 20, 20);
        Vector3 fifteen = new Vector3(15, 15, 15);
        Vector3 five = new Vector3(5, 5, 5);

        [Fact]
        public void Add()
        {
            Vector3[,] expected = MakeResult(size, size, twenty);
            FastMatrix<Vector3> matrix = MakeMatrix(size, size, fifteen);
            FastMatrix<Vector3> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Add(matrix, matrix2), expected);
        }

        [Fact]
        public void Subtract()
        {
            Vector3[,] expected = MakeResult(size, size, fifteen);
            FastMatrix<Vector3> matrix = MakeMatrix(size, size, twenty);
            FastMatrix<Vector3> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Subtract(matrix, matrix2), expected);
        }

        [Fact]
        public void Multiply()
        {
            Vector3[,] expected = MakeResult(size, size, new Vector3(375, 375, 375));
            FastMatrix<Vector3> matrix = MakeMatrix(size, size, fifteen);
            FastMatrix<Vector3> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Multiply(matrix, matrix2), expected);
        }

        [Fact]
        public void Transpose()
        {
            Vector3[,] expected = MakeResult(size, size, fifteen);
            FastMatrix<Vector3> matrix = MakeMatrix(size, size, fifteen);

            matrix[0, size - 1] = five;
            expected[size - 1, 0] = five;

            matrix = cpu.Transpose(matrix);

            VerifyResults(matrix, expected);
        }
    }
}
