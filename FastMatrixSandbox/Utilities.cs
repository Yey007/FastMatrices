
namespace FastMatrixOperations.Sandbox
{
    public static class Utilities
    {
        public static void FillMatrix<T>(FastMatrix<T> matrix, T value)
            where T: unmanaged
        {
            for(int i = 0, n = matrix.GetSize(0); i < n; i++)
            {
                for (int j = 0, m = matrix.GetSize(1); j < m; j++)
                {
                    matrix[i, j] = value;
                }
            }
        }
    }
}
