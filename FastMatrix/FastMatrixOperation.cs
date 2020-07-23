using System;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using FastMatrixOperations.Internal;
using System.Diagnostics;
using ILGPU.Runtime.Cuda;

namespace FastMatrixOperations
{
    /// <summary>
    /// The base class for CPU operators
    /// </summary>
    public interface ICpuOperator<T>
        where T: IOperatable<T>
    {
        public FastMatrix<T> Add(FastMatrix<T> one,
            FastMatrix<T> two);
        public FastMatrix<T> Subtract(FastMatrix<T> one,
            FastMatrix<T> two);
        public FastMatrix<T> Multiply(FastMatrix<T> one,
            FastMatrix<T> two);
        public FastMatrix<T> Transpose(
            FastMatrix<T> matrix);
    }

    /// <summary>
    /// Accesses the CPU for operations
    /// </summary>
    public class SingleThreadedOperator<T> : ICpuOperator<T>
        where T: IOperatable<T>
    {
        /// <summary>
        /// Adds two matrices on the CPU using a single thread
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public FastMatrix<T> Add(FastMatrix<T> one, FastMatrix<T> two)
        {
            if(one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
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
        public FastMatrix<T> Multiply(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }
            FastMatrix<T> returnMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));

            for (int i = 0; i < returnMatrix.GetSize(0); i++)
            {
                for (int j = 0; j < returnMatrix.GetSize(1); j++)
                {
                    T sum = one[i, 0].Multiply(two[0, j]);
                    for (int k = 1; k < one.GetSize(0); k++)
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
        public FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));

            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
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
        public FastMatrix<T> Transpose(FastMatrix<T> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            FastMatrix<T> returnMatrix = new FastMatrix<T>(matrix.GetSize(1), matrix.GetSize(0));
            for (int i = 0; i < matrix.GetSize(0); i++)
            {
                for (int j = 0; j < matrix.GetSize(1); j++)
                {
                    returnMatrix[j, i] = matrix[i, j];
                }
            }
            return returnMatrix;
        }
    }

    /// <summary>
    /// Accesses the CPU for operations, but operations run using multiple threads
    /// </summary>
    public class MultiThreadedOperator<T> : ICpuOperator<T>
        where T: IOperatable<T>
    {
        /// <summary>
        /// Adds two matrices on the CPU using multiple threads
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public FastMatrix<T> Add(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j].Add(two[i, j]);
                }
            });
            return fastMatrix;
        }

        /// <summary>
        /// Multiplies two matrices on the CPU using multiple threads
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the multiplication</returns>
        public FastMatrix<T> Multiply(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }

            FastMatrix<T> returnMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));

            Parallel.For(0, returnMatrix.GetSize(0), (i) =>
            {
                for (int j = 0; j < returnMatrix.GetSize(1); j++)
                {
                    T sum = one[i, 0].Multiply(two[0, j]);
                    for (int k = 1; k < one.GetSize(0); k++)
                    {
                        sum = sum.Add(one[i, k].Multiply(two[k, j]));
                    }
                    returnMatrix[i, j] = sum;
                }
            });
            return returnMatrix;
        }

        /// <summary>
        /// Subtracts two matrices on the CPU using multiple threads
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the subtraction (one - two)</returns>
        public FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j].Subtract(two[i, j]);
                }
            });
            return fastMatrix;
        }

        /// <summary>
        /// Transposes a matrix on the CPU using multiple threads
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The transposed matrix</returns>
        public FastMatrix<T> Transpose(FastMatrix<T> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            FastMatrix<T> returnMatrix = new FastMatrix<T>(matrix.GetSize(1), matrix.GetSize(0));

            Parallel.For(0, matrix.GetSize(0), (i) =>
            {
                for (int j = 0; j < matrix.GetSize(1); j++)
                {
                    returnMatrix[j, i] = matrix[i, j];
                }
            });

            return returnMatrix;
        }
    }

    /// <summary>
    /// Accesses the GPU for operations
    /// </summary>
    /// <typeparam name="T">The type this operator operates on.</typeparam>
    /// <remarks>
    /// Note: This is not always faster. There is a lot of overhead in copying information.
    /// </remarks>
    public class GPUOperator<T>
        where T : unmanaged, IGPUOperatable<T>
    {
        private static Accelerator accelerator;

        #region Preloaded kernels
        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUAddKernel;

        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
             GPUSubKernel;

        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUMultKernel;

        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>> GPUTransposeKernel;
        #endregion

        static GPUOperator()
        {
            accelerator = HardwareAcceleratorManager.GPUAccelerator;
            GPUAddKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>, 
                ArrayView2D<T>, ArrayView2D<T>>(GPUAdd);
            GPUSubKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>,
                ArrayView2D<T>, ArrayView2D<T>>(GPUSub);
            GPUMultKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>,
                ArrayView2D<T>, ArrayView2D<T>>(GPUMult);
            GPUTransposeKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>,
                ArrayView2D<T>>(GPUTranspose);
        }

        /// <summary>
        /// Adds two matrices on the GPU
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public BufferedFastMatrix<T> Add(BufferedFastMatrix<T> one, BufferedFastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }

            Stopwatch watch = Stopwatch.StartNew();
            MemoryBuffer2D<T> resultBuffer;

            one.CopyToGPU();
            two.CopyToGPU();
            Console.WriteLine($"Copy: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));
            Console.WriteLine($"Allocate: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            one.WaitForCopy(); //this function call is currently not required, 
                               //will come up with a better solution later but for now I'm just
                               //gonna leave it here
            two.WaitForCopy();
            Console.WriteLine($"Finish copy: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            GPUAddKernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();
            Console.WriteLine($"Execution: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();
            Console.WriteLine($"Copy back: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            BufferedFastMatrix<T> returnMatrix = new BufferedFastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Subtracts two matrices on the GPU
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the subtraction (one - two) </returns>
        public BufferedFastMatrix<T> Subtract(BufferedFastMatrix<T> one, BufferedFastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }

            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));
            
            one.WaitForCopy();
            two.WaitForCopy();

            GPUSubKernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            BufferedFastMatrix<T> returnMatrix = new BufferedFastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Multiplies two matrices on the GPU
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the multiplication</returns>
        public BufferedFastMatrix<T> Multiply(BufferedFastMatrix<T> one, BufferedFastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException(one.GetSize(0), one.GetSize(1), two.GetSize(0),
                    two.GetSize(1));
            }

            Stopwatch watch = Stopwatch.StartNew();
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();
            Console.WriteLine($"Copy: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), two.GetSize(1));
            Console.WriteLine($"Alloc: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            one.WaitForCopy();
            two.WaitForCopy();
            Console.WriteLine($"Finish copy: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            GPUMultKernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, 
                resultBuffer.View);

            accelerator.Synchronize();
            Console.WriteLine($"Execute: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();
            Console.WriteLine($"Copy back: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            BufferedFastMatrix<T> returnMatrix = new BufferedFastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Transposes a matrix on the GPU
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The transposed matrix</returns>
        public BufferedFastMatrix<T> Transpose(BufferedFastMatrix<T> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            Accelerator accelerator;
            MemoryBuffer2D<T> resultBuffer;

            matrix.CopyToGPU();

            accelerator = HardwareAcceleratorManager.GPUAccelerator;
            resultBuffer = accelerator.Allocate<T>(matrix.GetSize(1), matrix.GetSize(0));
            var kernel = GPUTransposeKernel;
            matrix.WaitForCopy();

            kernel(resultBuffer.Extent, matrix.buffer.View, resultBuffer.View);
            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            BufferedFastMatrix<T> returnMatrix = new BufferedFastMatrix<T>(tempArray);
            return returnMatrix;
        }

        //kernels
        #region Kernels
        private static void GPUAdd(Index2 index, ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            resView[index] = aView[index].Add(bView[index]);
        }
        private static void GPUSub(Index2 index, ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            resView[index] = aView[index].Subtract(bView[index]);
        }
        private static void GPUMult(Index2 index, ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            int x = index.X; //matrix one row
            int y = index.Y; //matrix two column
            
            //this needs to be done explicitly so we know where to start
            T sum = aView[x, 0].Multiply(bView[0, y]); 

            for (int i = 1; i < aView.Height; i++)
            {
                sum = sum.Add(aView[x, i].Multiply(bView[i, y]));
            }
            resView[index] = sum;
        }
        private static void GPUTranspose(Index2 index, ArrayView2D<T> originalView, 
            ArrayView2D<T> result)
        {
            result[index.Y, index.X] = originalView[index.X, index.Y];
        }
        #endregion
    }
}
