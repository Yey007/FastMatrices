
using FastMatrixOperations;

namespace FastMatrixOperations
{
    public struct IntOperator : ITypeOperator<int>
    {
        public int Add(int first, int second)
        {
            return first + second;
        }

        public int Multiply(int first, int second)
        {
            return first * second;
        }

        public int Subtract(int first, int second)
        {
            return first - second;
        }
    }

    public struct FloatOperator : ITypeOperator<float>
    {
        public float Add(float first, float second)
        {
            return first + second;
        }

        public float Multiply(float first, float second)
        {
            return first * second;
        }

        public float Subtract(float first, float second)
        {
            return first - second;
        }
    }

    public struct DoubleOperator : ITypeOperator<double>
    {
        public double Add(double first, double second)
        {
            return first + second;
        }

        public double Multiply(double first, double second)
        {
            return first + second;
        }

        public double Subtract(double first, double second)
        {
            return first + second;
        }
    }
}
