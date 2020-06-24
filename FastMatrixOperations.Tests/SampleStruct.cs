
namespace FastMatrixOperations.Tests
{
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

    public struct Vector3Operator : ITypeOperator<Vector3>
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