
namespace FastMatrixOperations.Samples.GPU
{
    static class Structs
    {
        static void Main()
        {
            GPUOperator<Vector3, Vector3Operator> op = new GPUOperator<Vector3, Vector3Operator>();

            //two 5*3 matrices
            BufferedFastMatrix<Vector3> one = new BufferedFastMatrix<Vector3>(5, 3);
            BufferedFastMatrix<Vector3> two = new BufferedFastMatrix<Vector3>(5, 3);

            Utilities.FillMatrix<Vector3>(one, new Vector3(10, 10, 10));
            Utilities.FillMatrix<Vector3>(two, new Vector3(5, 5, 5));

            op.Add(one, two);
        }
    }

    //sample struct
    public struct Vector3
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

        //define operators
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }

    //create a seperate struct that contains methods for the operators defined above
    //it must implement ITypeOperator in order to be used in the GPU
    //This is because operators cannot be resolved at runtime on the GPU
    //This is only necessary when using the GPU
    public struct Vector3Operator : IStructTypeOperator<Vector3>
    {
        public Vector3 Add(Vector3 first, Vector3 second)
        {
            return first + second;
        }

        public Vector3 Multiply(Vector3 first, Vector3 second)
        {
            return first * second;
        }

        public Vector3 Subtract(Vector3 first, Vector3 second)
        {
            return first - second;
        }
    }
}
