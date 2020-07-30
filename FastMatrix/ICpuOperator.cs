namespace FastMatrixOperations
{
    /// <summary>
    /// The base class for CPU operators
    /// </summary>
    public interface ICpuOperator<T>
        where T : IOperatable<T>
    {
        public FastMatrix<T> Add(FastMatrixBase<T> one, FastMatrixBase<T> two);
        public FastMatrix<T> Subtract(FastMatrixBase<T> one, FastMatrixBase<T> two);
        public FastMatrix<T> Multiply(FastMatrixBase<T> one, FastMatrixBase<T> two);
        public FastMatrix<T> Transpose(FastMatrixBase<T> matrix);
    }
}
