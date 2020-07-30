using Xunit;

namespace FastMatrixOperations.Tests
{
    public class TestBase<T>
    {
        protected const int size = 5;

        protected T[,] MakeResult(int rows, int columns, T value)
        {
            T[,] resultArray = new T[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return resultArray;
        }

        protected FastMatrix<T> MakeMatrix(int rows, int columns, T value)
        {
            T[,] resultArray = new T[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new FastMatrix<T>(resultArray);
        }

        protected BufferedFastMatrix<Tgpu> MakeBufferedMatrix<Tgpu>(int rows, int columns, Tgpu value)
            where Tgpu : unmanaged
        {
            Tgpu[,] resultArray = new Tgpu[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new BufferedFastMatrix<Tgpu>(resultArray);
        }

        protected void VerifyResults(FastMatrixBase<T> matrix, T[,] expected)
        {
            Assert.Equal(matrix.Rows, expected.GetLength(0));
            Assert.Equal(matrix.Columns, expected.GetLength(1));
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    Assert.Equal(expected[i, j], matrix[i, j]);
                }
            }
        }
    }
}
