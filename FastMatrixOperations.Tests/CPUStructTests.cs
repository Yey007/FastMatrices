using System;
using Xunit;
using System.Diagnostics;

namespace FastMatrixOperations.Tests
{
    public class CPUStructTests : TestBase<Vector3>
    {
        CPUOperator<Vector3> cpu = new CPUOperator<Vector3>();

        [Fact]
        public void Add()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(20, 20, 20));
            VerifyResults(AddOp(cpu, new Vector3(15, 15, 15), new Vector3(5, 5, 5)), 
                resultArray);
        }

        [Fact]
        public void Subtract()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(15, 15, 15));
            VerifyResults(SubtractionOp(cpu, new Vector3(20, 20, 20), new Vector3(5, 5, 5)), 
                resultArray);
        }

        [Fact]
        public void Multiply()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(750, 750, 750));
            VerifyResults(MultiplicationOp(cpu, new Vector3(15, 15, 15), new Vector3(10, 10, 10)), 
                resultArray);
        }

        [Fact]
        public void Transpose()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(10, 10, 10));
            VerifyResults(TransposeOp(cpu, resultArray, new Vector3(10, 10, 10), 
                new Vector3(5, 5, 5)), resultArray);
        }
    }
}
