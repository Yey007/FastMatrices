using System;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using FastMatrixOperations.Internal;
using System.Diagnostics;

namespace FastMatrixOperations
{
    /// <summary>
    /// The base class for CPU operators
    /// </summary>
    public abstract class CPUOperatorBase<T>
        where T: IOperatable<T>
    {
        public abstract FastMatrix<T> Add(FastMatrix<T> one,
            FastMatrix<T> two);
        public abstract FastMatrix<T> Subtract(FastMatrix<T> one,
            FastMatrix<T> two);
        public abstract FastMatrix<T> Multiply(FastMatrix<T> one,
            FastMatrix<T> two);
        public abstract FastMatrix<T> Transpose(
            FastMatrix<T> matrix);
        protected static T MultiplyArrays(T[] row, T[] column)
        {
            T sum = row[0].Multiply(column[0]);
            for (int i = 1; i < row.Length; i++)
            {
                sum = sum.Add(row[i].Multiply(column[i]));
            }

            return sum;
        }
    }

    /// <summary>
    /// Accesses the CPU for operations
    /// </summary>
    public class SingleThreadedOperator<T> : CPUOperatorBase<T>
        where T: IOperatable<T>
    {
        /// <summary>
        /// Adds two matrices on the CPU using a single thread
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public override FastMatrix<T> Add(FastMatrix<T> one, FastMatrix<T> two)
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
        public override FastMatrix<T> Multiply(FastMatrix<T> one, FastMatrix<T> two)
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
                    returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
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
        public override FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two)
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
        public override FastMatrix<T> Transpose(FastMatrix<T> matrix)
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
    public class MultiThreadedOperator<T> : CPUOperatorBase<T>
        where T: IOperatable<T>
    {
        /// <summary>
        /// Adds two matrices on the CPU using multiple threads
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the addition</returns>
        public override FastMatrix<T> Add(FastMatrix<T> one, FastMatrix<T> two)
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
        public override FastMatrix<T> Multiply(FastMatrix<T> one, FastMatrix<T> two)
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
                    returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
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
        public override FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two)
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
        public override FastMatrix<T> Transpose(FastMatrix<T> matrix)
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
        private static Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;

        #region Preloaded kernels
        private static Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>> 
            GPUAddKernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>, 
                ArrayView2D<T>>(GPUAdd);

        private static Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUAddFallbackKernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>,
                ArrayView2D<T>>(GPUAddFallback);

        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUMultKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>,
                ArrayView2D<T>, ArrayView2D<T>>(GPUMult);

        private static Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUSubKernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>,
                ArrayView2D<T>>(GPUSub);

        private static Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            GPUSubFallbackKernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>,
                ArrayView2D<T>>(GPUSubFallback);

        private static Action<Index2, ArrayView2D<T>, ArrayView2D<T>>
            GPUTransposeKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>,
                ArrayView2D<T>>(GPUTranspose);
        #endregion


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

            Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<T> resultBuffer;

            one.CopyToGPU();
            two.CopyToGPU();

            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {
                //kernel = GPUAddKernel;
                //config = SharedMemoryConfig.RequestDynamic<T>(groupSize);
                kernel = GPUAddFallbackKernel;
                config = SharedMemoryConfig.Empty;
            }
            else
            {
                kernel = GPUAddFallbackKernel;
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View);

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

            Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>> kernel;
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();

            kernel = GPUMultKernel;
            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), two.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

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

            Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();

            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {
                kernel = GPUSubKernel;
                config = SharedMemoryConfig.RequestDynamic<T>(groupSize);
            }
            else
            {
                kernel = GPUSubFallbackKernel;
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

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
        private static void GPUAdd(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<T>().As2DView(group);
            shared[group] = aView[group].Add(bView[group]);
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUAddFallback(ArrayView2D<T> aView, ArrayView2D<T> bView,
            ArrayView2D<T> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = aView[group].Add(bView[group]);
        }
        private static void GPUSub(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<T>().As2DView(group);
            shared[group] = aView[group].Subtract(bView[group]);
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUSubFallback(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = aView[group].Subtract(bView[group]);
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
