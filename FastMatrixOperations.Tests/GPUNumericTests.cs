using Xunit;

namespace FastMatrixOperations.Tests
{
    public class GPUNumericTests : TestBase<int>
    {
        GPUOperator<int> gpu = new GPUOperator<int>();

        [Fact]
        public void Add()
        {
            int[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(gpu, 15, 5), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            int[,] resultArray = MakeResult(size, size, 15);
            VerifyResults(SubtractionOp(gpu, 20, 5), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            int[,] resultArray = MakeResult(size, size, 750);
            VerifyResults(MultiplicationOp(gpu, 15, 10), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            int[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(gpu, resultArray, 10, 5), resultArray);
        }
    }
}
