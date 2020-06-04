using ILGPU;
using ILGPU.Runtime;
using System;
using System.Runtime.InteropServices;

namespace FastMatrixOperations
{
    /// <summary>
    /// A class that makes matrix operations easy and fast
    /// </summary>
    public class FastMatrix
    {
        /// <summary>
        /// Actually stores the values for the matrix.
        /// </summary>
        /// <remarks>
        /// This is a two dimensional array as opposed to a jagged array, which ensures that all elements have the same length.
        /// <para>See <a href="https://stackoverflow.com/questions/4648914/why-we-have-both-jagged-array-and-multidimensional-array">this post</a> for more details.</para>
        /// </remarks>
        public double[,] array2d;

        /// <summary>
        /// Indexer to make querries look nicer
        /// </summary>
        public double this[int row, int column]
        {
            get
            {
                return array2d[row, column];
            }

            set
            {
                array2d[row, column] = value;
            }
        }

        /// <summary>
        /// Creates a FastMatrix object with the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public FastMatrix(int rows, int columns)
        {
            array2d = new double[rows, columns];
        }

        /// <summary>
        /// Creates a new FastMatrix object from a jagged array.
        /// </summary>
        /// <param name="array">The jagged array to be converted into a FastMatrix</param>
        /// <remarks>Note: The constructor will throw an exception if all inner arrays do not have the same length.</remarks>
        public FastMatrix(double[][] array)
        {
            //make sure size is correct
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length != array[0].Length)
                {
                    throw new BadDimensionException("Array provided for conversion is jagged! Element at index " + i + " has length " + array[i].Length + " while baseline (at index 0) has length " + array[0].Length);
                }
            }

            array2d = new double[array.Length, array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
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
        public FastMatrix(double[,] array)
        {
            array2d = array;
        }

        /// <summary>
        /// Prints the current state of the matrix to the console.
        /// </summary>
        public void Print()
        {
            for (int i = 0; i < GetSize(0); i++)
            {
                Console.Write("[");
                for (int j = 0; j < GetSize(1); j++)
                {
                    if (j == GetSize(1) - 1)
                    {
                        Console.Write(array2d[i, j]);
                    }
                    else
                    {
                        Console.Write(array2d[i, j] + ", ");
                    }
                }
                Console.WriteLine("]");
            }
        }

        /// <summary>
        /// Get the size of the matrix in a certain dimension.
        /// </summary>
        /// <param name="dimension">-1 for total length, 0 for the number of rows, 1 for the number of columns.</param>
        /// <returns></returns>
        public int GetSize(int dimension)
        {
            if (dimension == -1)
            {
                return array2d.Length;
            }
            return array2d.GetLength(dimension);
        }

        public double[] GetRow(int row)
        {
            double[] rowData = new double[GetSize(1)];
            for (int i = 0; i < rowData.Length; i++)
            {
                rowData[i] = array2d[row, i];
            }

            return rowData;
        }

        public double[] GetColumn(int column)
        {
            double[] columnData = new double[GetSize(0)];
            for (int i = 0; i < columnData.Length; i++)
            {
                columnData[i] = array2d[i, column];
            }

            return columnData;
        }

        /*
        /// <summary>
        /// Internal use. Copies the array for this matrix to the GPU.
        /// </summary>
        /// <param name="gpu">The GPU to copy the array to.</param>
        /// <returns>The copied array (in GPU memory)</returns>
        public double[,] CopyToGPU(GPGPU gpu)
        {
            return gpu.CopyToDevice(this.array2d);
        }
        */
        public void CopyToGPU(Accelerator accelerator)
        {
            using (var buffer = accelerator.Allocate<double>(array2d.GetLength(0) + 32, array2d.GetLength(1)))
            {
                //copy to accelerator
                buffer.CopyFrom(array2d, new Index2(), new Index2(32, 0), new Index2(array2d.GetLength(0), array2d.GetLength(1)));
            }
        }
    }
}
