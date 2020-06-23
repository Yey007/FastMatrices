using Xunit;

namespace FastMatrixOperations.Tests
{
    public class ParallelStructTests : TestBase<Vector3>
    {
        ParallelOperator<Vector3> parallel = new ParallelOperator<Vector3>();

        [Fact]
        public void Add()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(20, 20, 20));
            VerifyResults(AddOp(parallel, new Vector3(15, 15, 15), new Vector3(5, 5, 5)), resultArray);
        }

        [Fact]
        public void Subtract()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(15, 15, 15));
            VerifyResults(SubtractionOp(parallel, new Vector3(20, 20, 20), new Vector3(5, 5, 5)), resultArray);
        }

        [Fact]
        public void Multiply()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(750, 750, 750));
            VerifyResults(MultiplicationOp(parallel, new Vector3(15, 15, 15), new Vector3(10, 10, 10)), resultArray);
        }

        [Fact]
        public void Transpose()
        {
            Vector3[,] resultArray = MakeResult(size, size, new Vector3(10, 10, 10));
            VerifyResults(TransposeOp(parallel, resultArray, new Vector3(10, 10, 10), new Vector3(5, 5, 5)), resultArray);
        }
    }
}
