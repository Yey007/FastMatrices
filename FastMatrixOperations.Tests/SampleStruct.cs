
namespace FastMatrixOperations.Tests
{
    public struct Vector3 : IGPUOperatable<Vector3>
    {
        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        int x, y, z;

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

        public override bool Equals(object obj)
        {
            Vector3 vector = (Vector3) obj;
            return (this.x == vector.x) && (this.y == vector.y) && (this.z == vector.z);
        }

        public Vector3 Add(Vector3 t)
        {
            return this + t;
        }

        public Vector3 Subtract(Vector3 t)
        {
            return this - t;
        }

        public Vector3 Multiply(Vector3 t)
        {
            return this * t;
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }
    }
}