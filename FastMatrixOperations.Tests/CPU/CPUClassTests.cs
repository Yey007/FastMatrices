using Xunit;

namespace FastMatrixOperations.Tests.CPU
{
    public class CPUClassTests : TestBase<Vector2>
    {
        SingleThreadedOperator<Vector2> cpu = new SingleThreadedOperator<Vector2>();
        Vector2 twenty = new Vector2(20, 20);
        Vector2 fifteen = new Vector2(15, 15);
        Vector2 five = new Vector2(5, 5);

        [Fact]
        public void Add()
        {
            Vector2[,] expected = MakeResult(size, size, twenty);
            FastMatrix<Vector2> matrix = MakeMatrix(size, size, fifteen);
            FastMatrix<Vector2> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Add(matrix, matrix2), expected);
        }

        [Fact]
        public void Subtract()
        {
            Vector2[,] expected = MakeResult(size, size, fifteen);
            FastMatrix<Vector2> matrix = MakeMatrix(size, size, twenty);
            FastMatrix<Vector2> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Subtract(matrix, matrix2), expected);
        }

        [Fact]
        public void Multiply()
        {
            Vector2[,] expected = MakeResult(size, size, new Vector2(375, 375));
            FastMatrix<Vector2> matrix = MakeMatrix(size, size, fifteen);
            FastMatrix<Vector2> matrix2 = MakeMatrix(size, size, five);
            VerifyResults(cpu.Multiply(matrix, matrix2), expected);
        }

        [Fact]
        public void Transpose()
        {
            Vector2[,] expected = MakeResult(size, size, fifteen);
            FastMatrix<Vector2> matrix = MakeMatrix(size, size, fifteen);

            matrix[0, size - 1] = five;
            expected[size - 1, 0] = five;

            matrix = cpu.Transpose(matrix);

            VerifyResults(matrix, expected);
        }
    }
}
