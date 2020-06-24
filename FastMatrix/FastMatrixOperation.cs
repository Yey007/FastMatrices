using System;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    /// <summary>
    /// The base class that all operators derive from
    /// </summary>
    public abstract class MatrixOperatorBase<T>
        where T: unmanaged
    {
        public abstract FastMatrix<T> Add(FastMatrix<T> one, FastMatrix<T> two);
        public abstract FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two);
        public abstract FastMatrix<T> Multiply(FastMatrix<T> one, FastMatrix<T> two);
        public abstract FastMatrix<T> Transpose(FastMatrix<T> matrix);
        protected static T MultiplyArrays(T[] row, T[] column)
        {
            T sum = (dynamic)row[0] * column[0];
            for (int i = 1; i < row.Length; i++)
            {
                sum += (dynamic) row[i] * column[i];
            }

            return sum;
        }
    }

    /// <summary>
    /// Accesses the CPU for operations
    /// </summary>
    public class CPUOperator<T> : MatrixOperatorBase<T>
        where T: unmanaged
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
                throw new BadDimensionException("The matrices to be added " +
                    "do not have the same sizes!");
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = (dynamic) one[i, j] + two[i, j];
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
                throw new BadDimensionException("Matrices to be multiplied do not have compliant " 
                    + "sizes! The number of columns in matrix one is " + one.GetSize(1) + " while "
                    + "the number of rows in matrix two is " + two.GetSize(0));
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
                throw new BadDimensionException("The matrices to be " +
                    "subtracted do not have the same sizes!");
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));

            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = (dynamic) one[i, j] - two[i, j];
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
    public class ParallelOperator<T> : MatrixOperatorBase<T>
        where T : unmanaged
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
                throw new BadDimensionException("The matrices to be " +
                    "added do not have the same sizes!");
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = (dynamic) one[i, j] + two[i, j];
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
                throw new BadDimensionException("Matrices to be multiplied do not have compliant " 
                    + "sizes! The number of columns in matrix one is " + one.GetSize(1) + " while " 
                    + "the number of rows in matrix two is " + two.GetSize(0));
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
                throw new BadDimensionException("The matrices to be " +
                    "subtracted do not have the same sizes!");
            }
            FastMatrix<T> fastMatrix = new FastMatrix<T>(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = (dynamic) one[i, j] - two[i, j];
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
    /// <typeparam name="TOperator">The associated operator. This is necessary as the GPU 
    /// cannot dynamically resolve operations.</typeparam>
    /// <remarks>
    /// Note: This is not always faster. There is a lot of overhead in copying information.
    /// <br></br>
    /// You can mitigate this overhead by starting copies early using 
    /// <see cref="FastMatrixOperations.FastMatrix.CopyToGPU"/> <br></br>
    /// </remarks>
    public class GPUOperator<T, TOperator> : MatrixOperatorBase<T>
        where T : unmanaged
        where TOperator: struct, ITypeOperator<T>
    {
        private Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;

        /// <summary>
        /// Adds two matrices on the GPU
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
                throw new BadDimensionException("The matrices to be " +
                    "added do not have the same sizes!");
            }

            Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>, TOperator> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            if (one.buffer == null)
            {
                one.CopyToGPU();
            }

            if (two.buffer == null)
            {
                two.CopyToGPU();
            }
            
            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>, 
                    ArrayView2D<T>, TOperator>(GPUAdd);
                config = SharedMemoryConfig.RequestDynamic<T>(groupSize);
            }
            else
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>, 
                    ArrayView2D<T>, TOperator>(GPUAddFallback);
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View, default);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            FastMatrix<T> returnMatrix = new FastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Multiplies two matrices on the GPU
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
                throw new BadDimensionException("Matrices to be multiplied do not have compliant " 
                    + "sizes! The number of columns in matrix one is " + one.GetSize(1) + " while " 
                    + "the number of rows in matrix two is " + two.GetSize(0));
            }

            Action<Index2, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>, TOperator> kernel;
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            if (one.buffer == null)
            {
                one.CopyToGPU();
            }

            if (two.buffer == null)
            {
                two.CopyToGPU();
            }

            kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>, 
                ArrayView2D<T>, ArrayView2D<T>, TOperator>(GPUMult);
            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), two.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View, 
                default);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            FastMatrix<T> returnMatrix = new FastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Subtracts two matrices on the GPU
        /// </summary>
        /// <param name="one">The first matrix</param>
        /// <param name="two">The second matrix</param>
        /// <returns>The result of the subtraction (one - two) </returns>
        public override FastMatrix<T> Subtract(FastMatrix<T> one, FastMatrix<T> two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be " +
                    "subtracted do not have the same sizes!");
            }

            Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>, TOperator> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            if (one.buffer == null)
            {
                one.CopyToGPU();
            }

            if (two.buffer == null)
            {
                two.CopyToGPU();
            }

            if (accelerator.AcceleratorType == AcceleratorType.Cuda)
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>, 
                    ArrayView2D<T>, TOperator>(GPUSub);
                config = SharedMemoryConfig.RequestDynamic<T>(groupSize);
            }
            else
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<T>, ArrayView2D<T>, 
                    ArrayView2D<T>, TOperator>(GPUSubFallback);
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<T>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View, default);

            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            FastMatrix<T> returnMatrix = new FastMatrix<T>(tempArray);
            return returnMatrix;
        }

        /// <summary>
        /// Transposes a matrix on the GPU
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>The transposed matrix</returns>
        public override FastMatrix<T> Transpose(FastMatrix<T> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            Accelerator accelerator;
            MemoryBuffer2D<T> resultBuffer;

            if (matrix.buffer == null)
            {
                matrix.CopyToGPU();
            }

            accelerator = HardwareAcceleratorManager.GPUAccelerator;
            resultBuffer = accelerator.Allocate<T>(matrix.GetSize(1), matrix.GetSize(0));
            var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<T>, 
                ArrayView2D<T>>(GPUTranspose);
            matrix.WaitForCopy();

            kernel(resultBuffer.Extent, matrix.buffer.View, resultBuffer.View);
            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            FastMatrix<T> returnMatrix = new FastMatrix<T>(tempArray);
            return returnMatrix;
        }

        //kernels
        #region Kernels
        private static void GPUAdd(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView, TOperator op)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<T>().As2DView(group);
            shared[group] = op.Add(aView[group], bView[group]);
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUAddFallback(ArrayView2D<T> aView, ArrayView2D<T> bView,
            ArrayView2D<T> resView, TOperator op)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = op.Add(aView[group], bView[group]);
        }
        private static void GPUSub(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView, TOperator op)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<T>().As2DView(group);
            shared[group] = op.Subtract(aView[group], bView[group]);
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUSubFallback(ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView, TOperator op)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = op.Subtract(aView[group], bView[group]);
        }
        private static void GPUMult(Index2 index, ArrayView2D<T> aView, ArrayView2D<T> bView, 
            ArrayView2D<T> resView, TOperator op)
        {
            int x = index.X; //matrix one row
            int y = index.Y; //matrix two column
            T sum = op.Multiply(aView[x, 0], bView[0, y]); //this needs to be done explicitly so we know where to start

            for (int i = 1; i < aView.Height; i++)
            {
                sum = op.Add(sum, op.Multiply(aView[x, i], bView[i, y]));
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
