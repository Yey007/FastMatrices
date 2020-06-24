using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System;
using System.Threading.Tasks;
using FastMatrixOperations.Internal;
using System.Collections.Generic;

namespace FastMatrixOperations
{
    /// <summary>
    /// An unbuffered fast matrix. It supports classes, but can only
    /// operate on the CPU.
    /// </summary>
    /// <typeparam name="T">The type this matrix should store</typeparam>
    public class UnbufferedFastMatrix<T>
    {
        protected T[,] array2d;

        /// <summary>
        /// Indexer to make querries look nicer
        /// </summary>
        public T this[int row, int column]
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
        /// Creates a UnbufferedFastMatrix object with the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public UnbufferedFastMatrix(int rows, int columns)
        {
            array2d = new T[rows, columns];
        }

        /// <summary>
        /// Creates a new UnbufferedFastMatrix object from a jagged array.
        /// </summary>
        /// <param name="array">The jagged array to be converted into a UnbufferedFastMatrix</param>
        /// <remarks>Note: The constructor will throw an exception if all 
        /// inner arrays do not have the same length.</remarks>
        public UnbufferedFastMatrix(T[][] array)
        {
            //make sure size is correct
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length != array[0].Length)
                {
                    throw new BadDimensionException("Array provided for conversion is jagged! " +
                        "Element at index " + i + " has length " + array[i].Length + " while " +
                        "baseline (at index 0) has length " + array[0].Length);
                }
            }

            array2d = new T[array.Length, array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    array2d[i, j] = array[i][j];
                }
            }
        }

        /// <summary>
        /// Creates a new UnbufferedFastMatrix object from a two dimensional array.
        /// </summary>
        /// <param name="array">A two dimensional array</param>
        public UnbufferedFastMatrix(T[,] array)
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
        /// <param name="dimension">-1 for total length, 0 for the number of rows, 
        /// 1 for the number of columns.</param>
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
        public T[] GetRow(int row)
        {
            T[] rowData = new T[GetSize(1)];
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
        public T[] GetColumn(int column)
        {
            T[] columnData = new T[GetSize(0)];
            for (int i = 0; i < columnData.Length; i++)
            {
                columnData[i] = array2d[i, column];
            }

            return columnData;
        }

        /// <summary>
        /// Override for default equals function
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnbufferedFastMatrix<T>);
        }

        /// <summary>
        /// Checks if this matrix is equal to another by looking at their contents
        /// </summary>
        /// <param name="matrix">The matrix to compare to</param>
        /// <returns>A bool representing wheter they are equal or not</returns>
        public bool Equals(UnbufferedFastMatrix<T> matrix)
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

            for (int i = 0; i < GetSize(0); i++)
            {
                for (int j = 0; j < GetSize(1); j++)
                {
                    if (!matrix[i, j].Equals(this[i, j]))
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
        public static bool operator ==(UnbufferedFastMatrix<T> one, UnbufferedFastMatrix<T> two)
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
        public static bool operator !=(UnbufferedFastMatrix<T> one, UnbufferedFastMatrix<T> two)
        {
            return !(one == two);
        }
    }

    /// <summary>
    /// A buffered fast matrix, It contains a memory buffer to allow copying
    /// to the GPU, but it only supports primatives and structs.
    /// </summary>
    /// <typeparam name="T">The type this matrix should store</typeparam>
    public class BufferedFastMatrix<T> : UnbufferedFastMatrix<T>
        where T: unmanaged
    {
        public MemoryBuffer2D<T> buffer { get; private set; }
        private Task copyTask;

        /// <summary>
        /// Creates a BufferedFastMatrix object with the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public BufferedFastMatrix(int rows, int columns) : base(rows, columns)
        {
            buffer = null;
        }

        /// <summary>
        /// Creates a new BufferedFastMatrix object from a jagged array.
        /// </summary>
        /// <param name="array">The jagged array to be converted into a BufferedFastMatrix</param>
        /// <remarks>Note: The constructor will throw an exception if all 
        /// inner arrays do not have the same length.</remarks>
        public BufferedFastMatrix(T[][] array) : base(array)
        {
            //set buffer to null so it can be set later/checked
            buffer = null;
        }

        /// <summary>
        /// Creates a new BufferedFastMatrix object from a two dimensional array.
        /// </summary>
        /// <param name="array">A two dimensional array</param>
        public BufferedFastMatrix(T[,] array) : base(array)
        {
            buffer = null;
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
        private unsafe void CopyToGPUWorker()
        {
            if(HardwareAcceleratorManager.GPUAccelerator.MemorySize < 
                (GetSize(0) * GetSize(1) * sizeof(T)))
            {
                throw new OutOfMemoryException("The GPU doesn't have enough " +
                    "memory to house an array of this size!");
            }

            var accelerator = HardwareAcceleratorManager.GPUAccelerator;

            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {
                var CPUAccelerator = new CPUAccelerator(accelerator.Context);
                var stream = accelerator.CreateStream();
                var pinnedCPUBuffer = CPUAccelerator.Allocate<T>(
                    new Index2(GetSize(0), GetSize(1)));

                buffer = accelerator.Allocate<T>(pinnedCPUBuffer.Extent);

                //copy to CPU buffer
                pinnedCPUBuffer.CopyFrom(array2d, Index2.Zero, Index2.Zero, 
                    pinnedCPUBuffer.Extent);

                //copy to GPU
                lock (buffer)
                {
                    buffer.CopyFrom(stream, pinnedCPUBuffer, Index2.Zero);
                }
                stream.Synchronize();
            }
            else
            {
                buffer = accelerator.Allocate<T>(GetSize(0), GetSize(1));
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
    }
}
