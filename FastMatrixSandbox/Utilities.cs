
namespace FastMatrixOperations.Sandbox
{
    public static class Utilities
    {
        public static void FillMatrix<T>(FastMatrixBase<T> matrix, T value)
            where T: IOperatable<T>
        {
            for(int i = 0, n = matrix.Rows; i < n; i++)
            {
                for (int j = 0, m = matrix.Columns; j < m; j++)
                {
                    matrix[i, j] = value;
                }
            }
        }
    }
}
