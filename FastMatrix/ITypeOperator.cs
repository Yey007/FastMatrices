
namespace FastMatrixOperations
{
    public interface ITypeOperator<T>
        where T: struct
    {
        public T Add(T first, T second);
        public T Subtract(T first, T second);
        public T Multiply(T first, T second);
    }
}
