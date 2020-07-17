
namespace FastMatrixOperations.Samples.GPU
{
    static class Structs
    {
        static void Main()
        {
            GPUOperator<Vector3> op = new GPUOperator<Vector3>();

            //two 5*3 matrices
            BufferedFastMatrix<Vector3> one = new BufferedFastMatrix<Vector3>(5, 3);
            BufferedFastMatrix<Vector3> two = new BufferedFastMatrix<Vector3>(5, 3);

            Utilities.FillMatrix(one, new Vector3(10, 10, 10));
            Utilities.FillMatrix(two, new Vector3(5, 5, 5));

            op.Add(one, two);
        }
    }

    //sample struct
    public struct Vector3 : IGPUOperatable<Vector3>
    {
        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        int x;
        int y;
        int z;

        public Vector3 Add(Vector3 t)
        {
            return new Vector3(x + t.x, y + t.y, z + t.z);
        }

        public Vector3 Subtract(Vector3 t)
        {
            return new Vector3(x - t.x, y - t.y, z - t.z);
        }

        public Vector3 Multiply(Vector3 t)
        {
            return new Vector3(x * t.x, y * t.y, z * t.z);
        }
    }
}
