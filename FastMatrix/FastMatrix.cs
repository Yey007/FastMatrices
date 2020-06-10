﻿using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Copies the matrix to GPU memory.
        /// </summary>
        /// <returns>The stream associated with copying the matrix</returns>
        /// <remarks>Runs asynchronously</remarks>
        public void CopyToGPU()
        {
            copyTask = new Task(CopyToGPUWorker);
            copyTask.Start();
        }

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

        public void WaitForCopy()
        {
            copyTask.Wait();
        }
    }
}
