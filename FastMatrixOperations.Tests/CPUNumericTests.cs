using System;
using Xunit;
using System.Diagnostics;

namespace FastMatrixOperations.Tests
{
    public class CPUNumericTests : TestBase<int>
    {
        CPUOperator<int> cpu = new CPUOperator<int>();

        [Fact]
        public void Add()
        {
            int[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(cpu, 15, 5), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            int[,] resultArray = MakeResult(size, size, 15);
            VerifyResults(SubtractionOp(cpu, 20, 5), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            int[,] resultArray = MakeResult(size, size, 750);
            VerifyResults(MultiplicationOp(cpu, 15, 10), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            int[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(cpu, resultArray, 10, 5), resultArray);
        }
    }
}
