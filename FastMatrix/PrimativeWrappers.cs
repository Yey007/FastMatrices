
namespace FastMatrixOperations
{
    public struct IntWrapper : IGPUOperatable<IntWrapper>
    {
        public int val;
        public IntWrapper Add(IntWrapper t)
        {
            return val + t;
        }

        public IntWrapper Multiply(IntWrapper t)
        {
            return val * t;
        }

        public IntWrapper Subtract(IntWrapper t)
        {
            return val - t;
        }

        public static implicit operator int(IntWrapper w)
        {
            return w.val;
        }

        public static implicit operator IntWrapper(int i)
        {
            IntWrapper w = new IntWrapper();
            w.val = i;
            return w;
        }

        public override string ToString()
        {
            return val.ToString();
        }
    }

    public struct FloatWrapper : IGPUOperatable<FloatWrapper>
    {
        public float val;
        public FloatWrapper Add(FloatWrapper t)
        {
            return val + t;
        }

        public FloatWrapper Multiply(FloatWrapper t)
        {
            return val * t;
        }

        public FloatWrapper Subtract(FloatWrapper t)
        {
            return val - t;
        }

        public static implicit operator float(FloatWrapper w)
        {
            return w.val;
        }

        public static implicit operator FloatWrapper(float i)
        {
            FloatWrapper w = new FloatWrapper();
            w.val = i;
            return w;
        }

        public override string ToString()
        {
            return val.ToString();
        }
    }

    public struct DoubleWrapper : IGPUOperatable<DoubleWrapper>
    {
        public double val;
        public DoubleWrapper Add(DoubleWrapper t)
        {
            return val + t;
        }

        public DoubleWrapper Multiply(DoubleWrapper t)
        {
            return val * t;
        }

        public DoubleWrapper Subtract(DoubleWrapper t)
        {
            return val - t;
        }

        public static implicit operator double(DoubleWrapper w)
        {
            return w.val;
        }

        public static implicit operator DoubleWrapper(double i)
        {
            DoubleWrapper w = new DoubleWrapper();
            w.val = i;
            return w;
        }

        public override string ToString()
        {
            return val.ToString();
        }
    }
}
