using System;

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
}
