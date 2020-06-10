using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System;
using System.Threading.Tasks;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    /// <summary>
    /// A class that makes matrix operations easy and fast
    /// </summary>
    /// <remarks>See <seealso cref="FastMatrixOperations.MatrixOperatorBase"/> and children for doing operations</remarks>
    public class FastMatrix
    {
        /// <summary>
        /// Actually stores the values for the matrix.
        /// </summary>
        /// <remarks>
        /// This is a two dimensional array as opposed to a jagged array, which ensures that all elements have the same length.
        /// <para>See <a href="https://stackoverflow.com/questions/4648914/why-we-have-both-jagged-array-and-multidimensional-array">this post</a> for more details.</para>
        /// </remarks>
        private double[,] array2d;
        public MemoryBuffer2D<double> buffer { get; private set; }
        private Task copyTask;

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
            buffer = null;
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

            //set buffer to null so it can be set later/checked
            buffer = null;
        }

        /// <summary>
        /// Creates a new FastMatrix object from a two dimensional array.
        /// </summary>
        /// <param name="array">A two dimensional array</param>
        public FastMatrix(double[,] array)
        {
            array2d = array;
            buffer = null;
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

        /// <summary>
        /// Gets a specific row of the matrix as an array
        /// </summary>
        /// <param name="row">Which row to get (0-based top to bottom)</param>
        /// <returns>The row as an array</returns>
        public double[] GetRow(int row)
        {
            double[] rowData = new double[GetSize(1)];
            for (int i = 0; i < rowData.Length; i++)
            {
                rowData[i] = array2d[row, i];
            }

            return rowData;
        }

        /// <summary>
        /// Gets a specific column of the matrix as an array
        /// </summary>
        /// <param name="row">Which column to get (0-based left to right)</param>
        /// <returns>The column as an array</returns>
        public double[] GetColumn(int column)
        {
            double[] columnData = new double[GetSize(0)];
            for (int i = 0; i < columnData.Length; i++)
            {
                columnData[i] = array2d[i, column];
            }

            return columnData;
        }

        /// <summary>
        /// Copies the matrix to GPU memory.
        /// </summary>
        /// <returns>The stream associated with copying the matrix</returns>
        /// <remarks>
        /// Runs asynchronously.
        /// You can wait for the task to finish by calling <seealso cref="WaitForCopy"/>
        /// </remarks>
        public void CopyToGPU()
        {
            copyTask = new Task(CopyToGPUWorker);
            copyTask.Start();
        }

        /// <summary>
        /// The actual function ran from for <see cref="CopyToGPU"/>
        /// </summary>
        private void CopyToGPUWorker()
        {
            if(HardwareAcceleratorManager.GPUAccelerator.MemorySize < GetSize(0) * GetSize(1) * sizeof(double))
            {
                Console.WriteLine("Out of memory");
                throw new OutOfMemoryException("The GPU doesn't have enough memory to house an array of this size!");
            }

            var accelerator = HardwareAcceleratorManager.GPUAccelerator;

            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {


                var CPUAccelerator = new CPUAccelerator(accelerator.Context);
                var stream = accelerator.CreateStream();
                var pinnedCPUBuffer = CPUAccelerator.Allocate<double>(new Index2(GetSize(0), GetSize(1)));
                buffer = accelerator.Allocate<double>(pinnedCPUBuffer.Extent);

                //copy to CPU buffer
                pinnedCPUBuffer.CopyFrom(array2d, Index2.Zero, Index2.Zero, pinnedCPUBuffer.Extent);

                //copy to GPU
                lock (buffer)
                {
                    buffer.CopyFrom(stream, pinnedCPUBuffer, Index2.Zero);
                }
                stream.Synchronize();
            }
            else
            {
                buffer = accelerator.Allocate<double>(GetSize(0), GetSize(1));
                buffer.CopyFrom(array2d, Index2.Zero, Index2.Zero, buffer.Extent);
            }
        }

        /// <summary>
        /// Waits for <see cref="CopyToGPU"/> to finsih if running
        /// </summary>
        public void WaitForCopy()
        {
            if (copyTask != null && copyTask.Status == TaskStatus.Running)
            {
                copyTask.Wait();
            }
        }

        /// <summary>
        /// Override for default equals function
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FastMatrix);
        }

        /// <summary>
        /// Checks if this matrix is equal to another by looking at their contents
        /// </summary>
        /// <param name="matrix">The matrix to compare to</param>
        /// <returns>A bool representing wheter they are equal or not</returns>
        public bool Equals(FastMatrix matrix)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(matrix, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, matrix))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != matrix.GetType())
            {
                return false;
            }

            //if sizes aren't same return false
            if ((GetSize(0) != matrix.GetSize(0)) || (GetSize(1) != matrix.GetSize(1)))
            {
                return false;
            }

            for(int i = 0; i < GetSize(0); i++)
            {
                for (int j = 0; j < GetSize(1); j++)
                {
                    if(matrix[i, j] != this[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Hashes the array based on it's contents
        /// </summary>
        /// <returns>An int representing the hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(array2d);
        }

        /// <summary>
        /// Operator for comparing two matrices
        /// </summary>
        /// <param name="one">First matrix</param>
        /// <param name="two">Second matrix</param>
        /// <returns>Wheter they are equal or not</returns>
        public static bool operator ==(FastMatrix one, FastMatrix two)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(one, null))
            {
                if (Object.ReferenceEquals(two, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return one.Equals(two);
        }

        /// <summary>
        /// Basically just <see cref="operator ==(FastMatrix, FastMatrix)"/> but uno reverse
        /// </summary>
        public static bool operator !=(FastMatrix one, FastMatrix two)
        {
            return !(one == two);
        }
    }
}
