using Xunit;

namespace FastMatrixOperations.Tests
{
    public class GPUTests : TestBase
    {
        GPUOperator gpu = new GPUOperator();

        [Fact]
        public void Add()
        {
            double[,] resultArray = MakeResult(size, size, 20);
            VerifyResults(AddOp(gpu), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(SubtractionOp(gpu), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            double[,] resultArray = MakeResult(size, size, 500);
            VerifyResults(MultiplicationOp(gpu), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            double[,] resultArray = MakeResult(size, size, 10);
            VerifyResults(TransposeOp(gpu, resultArray), resultArray);
        }
    }
}
