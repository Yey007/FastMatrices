using System;
using System.Text;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    /// <summary>
    /// An unbuffered fast matrix. It supports classes, but can only
    /// operate on the CPU.
    /// </summary>
    /// <typeparam name="T">The type this matrix should store</typeparam>
    public class FastMatrix<T> : FastMatrixBase<T>
    {
        protected T[,] array2d;

        /// <summary>
        /// Indexer to make querries look nicer
        /// </summary>
        public override T this[int row, int column]
        {
            get => array2d[row, column];
            set => array2d[row, column] = value;
        }

        /// <summary>
        /// Creates a FastMatrix object with the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public FastMatrix(int rows, int columns) : base(rows, columns)
        {
            array2d = new T[rows, columns];
        }

        /// <summary>
        /// Creates a new FastMatrix object from a jagged array.
        /// </summary>
        /// <param name="array">The jagged array to be converted into a FastMatrix</param>
        /// <remarks>Note: The constructor will throw an exception if all 
        /// inner arrays do not have the same length.</remarks>
        public FastMatrix(T[][] array) : this(array.Length, array[0].Length)
        {

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length != array[0].Length)
                {
                    throw new JaggedArrayException(array[0].Length, array[i].Length, i);
                }

                for (int j = 0; j < array[i].Length; j++)
                {
                    array2d[i, j] = array[i][j];
                }
            }
        }

        /// <summary>
        /// Creates a new FastMatrix object from a two dimensional array.
        /// </summary>
        /// <param name="array">A two dimensional array</param>
        public FastMatrix(T[,] array) : base(array.GetLength(0), array.GetLength(1))
        {
            array2d = array;
        }

        /// <summary>
        /// Hashes the array based on it's contents
        /// </summary>
        /// <returns>An int representing the hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(array2d);
        }
    }
}
