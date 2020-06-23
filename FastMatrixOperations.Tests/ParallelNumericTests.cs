using Xunit;

namespace FastMatrixOperations.Tests
{
    public class ParallelNumericTests : TestBase<int>
    {
        ParallelOperator<int> parallel = new ParallelOperator<int>();

        [Fact]
        public void Add()
        {
            int[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(parallel, 15, 5), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            int[,] resultArray = MakeResult(size, size, 15);
            VerifyResults(SubtractionOp(parallel, 20, 5), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            int[,] resultArray = MakeResult(size, size, 750);
            VerifyResults(MultiplicationOp(parallel, 15, 10), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            int[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(parallel, resultArray, 10, 5), resultArray);
        }
    }
}
