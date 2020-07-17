using System;
using Xunit;
using System.Diagnostics;

namespace FastMatrixOperations.Tests.CPU
{
    public class CPUNumericTests : TestBase<IntWrapper>
    {
        SingleThreadedOperator<IntWrapper> cpu = new SingleThreadedOperator<IntWrapper>();

        [Fact]
        public void Add()
        {
            IntWrapper[,] expected = MakeResult(size, size, 20);
            FastMatrix<IntWrapper> matrix = MakeMatrix(size, size, 15);
            FastMatrix<IntWrapper> matrix2 = MakeMatrix(size, size, 5);
            VerifyResults(cpu.Add(matrix, matrix2), expected);
        }

        [Fact]
        public void Subtract()
        {
            IntWrapper[,] expected = MakeResult(size, size, 10);
            FastMatrix<IntWrapper> matrix = MakeMatrix(size, size, 15);
            FastMatrix<IntWrapper> matrix2 = MakeMatrix(size, size, 5);
            VerifyResults(cpu.Subtract(matrix, matrix2), expected);
        }

        [Fact]
        public void Multiply()
        {
            IntWrapper[,] expected = MakeResult(size, size, 375);
            FastMatrix<IntWrapper> matrix = MakeMatrix(size, size, 15);
            FastMatrix<IntWrapper> matrix2 = MakeMatrix(size, size, 5);
            VerifyResults(cpu.Multiply(matrix, matrix2), expected);
        }

        [Fact]
        public void Transpose()
        {
            IntWrapper[,] expected = MakeResult(size, size, 10);
            FastMatrix<IntWrapper> matrix = MakeMatrix(size, size, 10);

            matrix[0, size - 1] = 5;
            expected[size - 1, 0] = 5;

            matrix = cpu.Transpose(matrix);

            VerifyResults(matrix, expected);
        }
    }
}
