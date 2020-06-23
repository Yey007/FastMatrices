using System;
using Xunit;

namespace FastMatrixOperations.Tests
{
    public class TestBase<T>
        where T: unmanaged
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

        protected void VerifyResults(FastMatrix<T> matrix, T[,] expected)
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

        //expect a + b
        protected FastMatrix<int> AddOp2(GPUOperator<int> matrixOperator, int a, int b)
        {
            FastMatrix<int> one = MakeMatrix2(size, size, a);
            FastMatrix<int> two = MakeMatrix2(size, size, b);

            return matrixOperator.Add2(one, two);
        }

        protected FastMatrix<int> MakeMatrix2(int rows, int columns, int value)
        {
            int[,] resultArray = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new FastMatrix<int>(resultArray);
        }

        //expect a + b
        protected FastMatrix<T> AddOp(MatrixOperatorBase<T> matrixOperator, T a, T b)
        {
            FastMatrix<T> one = MakeMatrix(size, size, a);
            FastMatrix<T> two = MakeMatrix(size, size, b);

            return matrixOperator.Add(one, two);
        }

        //expect a - b
        protected FastMatrix<T> SubtractionOp(MatrixOperatorBase<T> matrixOperator, T a, T b)
        {
            FastMatrix<T> one = MakeMatrix(size, size, a);
            FastMatrix<T> two = MakeMatrix(size, size, b);

            return matrixOperator.Subtract(one, two);
        }

        //expect a*b*size
        protected FastMatrix<T> MultiplicationOp(MatrixOperatorBase<T> matrixOperator, T a, T b)
        {

            FastMatrix<T> one = MakeMatrix(size, size, a);
            FastMatrix<T> two = MakeMatrix(size, size, b);

            return matrixOperator.Multiply(one, two);
        }


        protected FastMatrix<T> TransposeOp(MatrixOperatorBase<T> matrixOperator, T[,] allocatedResult, T a, T b)
        {
            FastMatrix<T> one = MakeMatrix(size, size, a);

            one[size - 1, 0] = b;
            allocatedResult[0, size - 1] = b;

            return matrixOperator.Transpose(one);
        }
    }
}
