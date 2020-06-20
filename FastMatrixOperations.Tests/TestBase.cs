using System;
using Xunit;

namespace FastMatrixOperations.Tests
{
    public class TestBase
    {
        protected const int size = 5;

        protected double[,] MakeResult(int rows, int columns, double value)
        {
            double[,] resultArray = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return resultArray;
        }

        protected FastMatrix MakeMatrix(int rows, int columns, double value)
        {
            double[,] resultArray = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    resultArray[i, j] = value;
                }
            }

            return new FastMatrix(resultArray);
        }

        protected void VerifyResults(FastMatrix matrix, double[,] expected)
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

        protected FastMatrix AddOp(MatrixOperatorBase matrixOperator)
        {
            FastMatrix one = MakeMatrix(size, size, 10);
            FastMatrix two = MakeMatrix(size, size, 10);

            return matrixOperator.Add(one, two);
        }

        protected FastMatrix SubtractionOp(MatrixOperatorBase matrixOperator)
        {
            FastMatrix one = MakeMatrix(size, size, 20);
            FastMatrix two = MakeMatrix(size, size, 10);

            return matrixOperator.Subtract(one, two);
        }

        protected FastMatrix MultiplicationOp(MatrixOperatorBase matrixOperator)
        {

            FastMatrix one = MakeMatrix(size, size, 10);
            FastMatrix two = MakeMatrix(size, size, 10);

            return matrixOperator.Multiply(one, two);
        }

        protected FastMatrix TransposeOp(MatrixOperatorBase matrixOperator, double[,] allocatedResult)
        {
            FastMatrix one = MakeMatrix(size, size, 10);

            one[size - 1, 0] = 5;
            allocatedResult[0, size - 1] = 5;

            return matrixOperator.Transpose(one);
        }
    }
}
