using ILGPU.Runtime;
using FastMatrixOperations.Internal;
using System;

namespace FastMatrixOperations
{
    public class BufferedFastMatrix<T> : FastMatrixBase<T>
        where T: unmanaged
    {
        public ExchangeBuffer2D<T> buffer { get; private set; } = null;
        private AcceleratorStream stream = null;
        private bool IsCopying = false;

        public override T this[int row, int column] 
        { 
            get => buffer[(row, column)];
            set => buffer[(row, column)] = value;
        }

        /// <summary>
        /// Creates a BufferedFastMatrix object with the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public BufferedFastMatrix(int rows, int columns) : base(rows, columns)
        {
            var accelerator = HardwareAcceleratorManager.GPUAccelerator;
            stream = accelerator.CreateStream();
            buffer = accelerator.AllocateExchangeBuffer<T>((Rows, Columns));
        }

        /// <summary>
        /// Creates a new BufferedFastMatrix object from a jagged array.
        /// </summary>
        /// <param name="array">The jagged array to be converted into a BufferedFastMatrix</param>
        /// <remarks>Note: The constructor will throw an exception if all 
        /// inner arrays do not have the same length.</remarks>
        public BufferedFastMatrix(T[][] array) : this(array.Length, array[0].Length) 
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length != array[0].Length)
                {
                    throw new JaggedArrayException(array[0].Length, array[i].Length, i);
                }

                for (int j = 0; j < array[i].Length; j++)
                {
                    buffer[(i, j)] = array[i][j];
                }
            }
        }

        /// <summary>
        /// Creates a new BufferedFastMatrix object from a two dimensional array.
        /// </summary>
        /// <param name="array">A two dimensional array</param>
        public BufferedFastMatrix(T[,] array) : this(array.GetLength(0), array.GetLength(1)) 
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    buffer[(i, j)] = array[i, j];
                }
            }
        }

        ~BufferedFastMatrix()
        {
            buffer.Dispose();
            stream.Dispose();
        }

        public void CopyToGPU()
        {
            if (!IsCopying)
            {
                buffer.CopyToAccelerator(stream);
                IsCopying = true;
            }
        }

        /// <summary>
        /// Waits for <see cref="CopyToGPU"/> to finish
        /// </summary>
        public void WaitForCopy()
        {
            stream.Synchronize();
            IsCopying = false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(buffer.Span.ToArray());
        }
    }
}
