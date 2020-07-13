
namespace FastMatrixOperations.Samples.GPU
{
    static class Copy
    {
        //you do not have to opy, but the earlier you start it the better
        static void GPUCopy()
        {
            GPUOperator<int, IntOperator> op = new GPUOperator<int, IntOperator>();
            BufferedFastMatrix<int> matrix = new BufferedFastMatrix<int>(5, 5);

            //start copying to the GPU
            matrix.CopyToGPU();

            //do other stuff here...

            //all operations on the GPU will automatically finish the copy
            //when starting an operation
            op.Transpose(matrix);
        }
    }
}
