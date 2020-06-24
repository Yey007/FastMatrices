using System;
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

        protected UnbufferedFastMatrix<T> MakeUnbufferedMatrix(int rows, int columns, T value)
        {
            T[,] resultArray = new T[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new UnbufferedFastMatrix<T>(resultArray);
        }

        protected BufferedFastMatrix<TUnmanaged> MakeBufferedMatrix<TUnmanaged>(int rows, int columns, TUnmanaged value)
            where TUnmanaged : unmanaged
        {
            TUnmanaged[,] resultArray = new TUnmanaged[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new BufferedFastMatrix<TUnmanaged>(resultArray);
        }

        protected void VerifyResults(UnbufferedFastMatrix<T> matrix, T[,] expected)
        {
            Assert.Equal(matrix.GetSize(0), expected.GetLength(0));
            Assert.Equal(matrix.GetSize(1), expected.GetLength(1));
            for (int i = 0; i < matrix.GetSize(0); i++)
            {
                for (int j = 0; j < matrix.GetSize(1); j++)
                {
                    Assert.Equal(expected[i, j], matrix[i, j]);
                }
            }
        }
    }
}
