using System;
using Xunit;
using System.Diagnostics;

namespace FastMatrixOperations.Tests
{
    public class CPUTests : TestBase
    {
        CPUOperator cpu = new CPUOperator();

        [Fact]
        public void Add()
        {
            double[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(cpu), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(SubtractionOp(cpu), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            double[,] resultArray = MakeResult(size, size, 500);
            VerifyResults(MultiplicationOp(cpu), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(cpu, resultArray), resultArray);
        }
    }
}
