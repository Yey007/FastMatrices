using System;
using ILGPU;
using ILGPU.Runtime;
using FastMatrixOperations.Internal;
using System.Diagnostics;

namespace FastMatrixOperations
{
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

        private static Action<KernelConfig, ArrayView2D<T>, ArrayView2D<T>, ArrayView2D<T>>
            AddSharedKernel;
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
            AddSharedKernel = accelerator.LoadStreamKernel<ArrayView2D<T>,
                ArrayView2D<T>, ArrayView2D<T>>(GPUAddShared);
        }

        public unsafe BufferedFastMatrix<T> AddShared(BufferedFastMatrix<T> one, BufferedFastMatrix<T> two)
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

            MemoryBuffer2D<T> resultBuffer;
            one.CopyToGPU();
            two.CopyToGPU();

            resultBuffer = accelerator.Allocate<T>(one.Rows, one.Columns);

            one.WaitForCopy();
            two.WaitForCopy();

            KernelConfig config = new KernelConfig(accelerator.MaxGridSize.X, accelerator.MaxNumThreadsPerGroup);
            AddSharedKernel(config, one.buffer.View, two.buffer.View, resultBuffer);
            Console.WriteLine(accelerator.MaxGridSize.X);
            accelerator.Synchronize();

            var tempArray = resultBuffer.GetAs2DArray();
            accelerator.Synchronize();

            BufferedFastMatrix<T> returnMatrix = new BufferedFastMatrix<T>(tempArray);
            return returnMatrix;
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
            if ((one.Rows != two.Rows) || (one.Columns != two.Columns))
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }

            MemoryBuffer2D<T> resultBuffer;

            one.CopyToGPU();
            two.CopyToGPU();

            resultBuffer = accelerator.Allocate<T>(one.Rows, one.Columns);

            one.WaitForCopy(); //this function call is currently not required, 
                               //will come up with a better solution later but for now I'm just
                               //gonna leave it here
            two.WaitForCopy();

            GPUAddKernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View);
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
            if ((one.Rows != two.Rows) || (one.Columns != two.Columns))
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }

            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();

            resultBuffer = accelerator.Allocate<T>(one.Rows, one.Columns);
            
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
            if (one.Columns != two.Rows)
            {
                throw new BadDimensionException(one.Rows, one.Columns, two.Rows,
                    two.Columns);
            }

            Stopwatch watch = Stopwatch.StartNew();
            MemoryBuffer2D<T> resultBuffer;

            //start tasks
            one.CopyToGPU();
            two.CopyToGPU();

            resultBuffer = accelerator.Allocate<T>(one.Rows, two.Columns);

            one.WaitForCopy();
            two.WaitForCopy();

            GPUMultKernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, 
                resultBuffer.View);
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
            resultBuffer = accelerator.Allocate<T>(matrix.Columns, matrix.Rows);
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

        private static void GPUAddShared(ArrayView2D<T> aView, ArrayView2D<T> bView,
            ArrayView2D<T> resView)
        {
            
        }   
        #endregion
    }
}
