using System;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    /// <summary>
    /// Accesses the CPU for operations
    /// </summary>
    public class SingleThreadedOperator<T> : ICpuOperator<T>
        where T : IOperatable<T>
    {
        /// <summary>
        /// Adds two matrices on the CPU using a single thread
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public FastMatrix<T> Add(FastMatrixBase<T> one, FastMatrixBase<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.Rows != two.Rows) || (one.Columns != two.Columns))
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.Rows, two.Columns);
            for (int i = 0; i < one.Rows; i++)
            {
                for (int j = 0; j < one.Columns; j++)
                {
                    fastMatrix[i, j] = one[i, j].Add(two[i, j]);
                }
            }
            return fastMatrix;
        }

        /// <summary>
        /// Multiplies two matrices on the CPU using a single thread
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the multiplication</returns>
        public FastMatrix<T> Multiply(FastMatrixBase<T> one, FastMatrixBase<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.Columns != two.Rows)
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }
            FastMatrix<T> returnMatrix = new FastMatrix<T>(one.Rows, two.Columns);

            for (int i = 0; i < returnMatrix.Rows; i++)
            {
                for (int j = 0; j < returnMatrix.Columns; j++)
                {
                    T sum = one[i, 0].Multiply(two[0, j]);
                    for (int k = 1; k < one.Rows; k++)
                    {
                        sum = sum.Add(one[i, k].Multiply(two[k, j]));
                    }
                    returnMatrix[i, j] = sum;
                }
            }
            return returnMatrix;
        }

        /// <summary>
        /// Subtracts two matrices on the CPU using a single thread
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the subtraction (one - two)</returns>
        public FastMatrix<T> Subtract(FastMatrixBase<T> one, FastMatrixBase<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.Rows != two.Rows) || (one.Columns != two.Columns))
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.Rows, two.Columns);

            for (int i = 0; i < one.Rows; i++)
            {
                for (int j = 0; j < one.Columns; j++)
                {
                    fastMatrix[i, j] = one[i, j].Subtract(two[i, j]);
                }
            }
            return fastMatrix;
        }

        /// <summary>
        /// Transposes a matrix on the CPU using a single thread
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The result of the transpose</returns>
        public FastMatrix<T> Transpose(FastMatrixBase<T> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            FastMatrix<T> returnMatrix = new FastMatrix<T>(matrix.Columns, matrix.Rows);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    returnMatrix[j, i] = matrix[i, j];
                }
            }
            return returnMatrix;
        }
    }
}
