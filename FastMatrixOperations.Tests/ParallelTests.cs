using Xunit;

namespace FastMatrixOperations.Tests
{
    public class ParallelTests : TestBase
    {
        ParallelOperator parallel = new ParallelOperator();

        [Fact]
        public void Add()
        {
            double[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(parallel), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(SubtractionOp(parallel), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            double[,] resultArray = MakeResult(size, size, 500);
            VerifyResults(MultiplicationOp(parallel), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(parallel, resultArray), resultArray);
        }
    }
}
