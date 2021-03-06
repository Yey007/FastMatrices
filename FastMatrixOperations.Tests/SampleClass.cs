﻿
namespace FastMatrixOperations.Tests
{
    public class Vector2 : IOperatable<Vector2>
    {
        int x, y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x + right.x, left.y + right.y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x - right.x, left.y - right.y);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x * right.x, left.y * right.y);
        }

        public override bool Equals(object obj)
        {
            Vector2 vector = obj as Vector2;
            return (this.x == vector.x) && (this.y == vector.y);
        }

        public Vector2 Add(Vector2 t)
        {
            return this + t;
        }

        public Vector2 Subtract(Vector2 t)
        {
            return this - t;
        }

        public Vector2 Multiply(Vector2 t)
        {
            return this * t;
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }
    }
}
