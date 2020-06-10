using System;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    public abstract class MatrixOperatorBase
    {
        public abstract FastMatrix Add(FastMatrix one, FastMatrix two);
        public abstract FastMatrix Subtract(FastMatrix one, FastMatrix two);
        public abstract FastMatrix Multiply(FastMatrix one, FastMatrix two);
        public abstract FastMatrix Transpose(FastMatrix matrix);
        protected static double MultiplyArrays(double[] row, double[] column)
        {
            double sum = 0;
            for (int i = 0; i < row.Length; i++)
            {
                sum += row[i] * column[i];
            }

            return sum;
        }
    }

    public class CPUOperator : MatrixOperatorBase
    {
        public override FastMatrix Add(FastMatrix one, FastMatrix two)
        {
            if(one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }
            FastMatrix fastMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));
            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j] + two[i, j];
                }
            }
            return fastMatrix;
        }

        public override FastMatrix Multiply(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
            }

            FastMatrix returnMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));

            for (int i = 0; i < returnMatrix.GetSize(0); i++)
            {
                for (int j = 0; j < returnMatrix.GetSize(1); j++)
                {
                    returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
                }
            }
            return returnMatrix;
        }

        public override FastMatrix Subtract(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }
            FastMatrix fastMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));

            for (int i = 0; i < one.GetSize(0); i++)
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j] - two[i, j];
                }
            }
            return fastMatrix;
        }

        public override FastMatrix Transpose(FastMatrix matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            FastMatrix returnMatrix = new FastMatrix(matrix.GetSize(1), matrix.GetSize(0));
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

    public class ParallelOperator : MatrixOperatorBase
    {
        public override FastMatrix Add(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }
            FastMatrix fastMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j] + two[i, j];
                }
            });
            return fastMatrix;
        }

        public override FastMatrix Multiply(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
            }

            FastMatrix returnMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));

            Parallel.For(0, returnMatrix.GetSize(0), (i) =>
            {
                for (int j = 0; j < returnMatrix.GetSize(1); j++)
                {
                    returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
                }
            });
            return returnMatrix;
        }

        public override FastMatrix Subtract(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }
            FastMatrix fastMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));
            Parallel.For(0, one.GetSize(0), i =>
            {
                for (int j = 0; j < one.GetSize(1); j++)
                {
                    fastMatrix[i, j] = one[i, j] - two[i, j];
                }
            });
            return fastMatrix;
        }

        public override FastMatrix Transpose(FastMatrix matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            FastMatrix returnMatrix = new FastMatrix(matrix.GetSize(1), matrix.GetSize(0));

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

    public class GPUOperator : MatrixOperatorBase
    {
        private Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;
        public GPUOperator()
        {
            HardwareAcceleratorManager.StartInit();
        }

        public override FastMatrix Add(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }

            Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<double> resultBuffer;

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
                kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAdd);
                config = SharedMemoryConfig.RequestDynamic<double>(groupSize);
            }
            else
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAddFallback);
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            FastMatrix returnMatrix = new FastMatrix(resultBuffer.GetAs2DArray());
            return returnMatrix;
        }

        public override FastMatrix Multiply(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if (one.GetSize(1) != two.GetSize(0))
            {
                throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
            }

            Action<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
            MemoryBuffer2D<double> resultBuffer;

            //start tasks
            if (one.buffer == null)
            {
                one.CopyToGPU();
            }

            if (two.buffer == null)
            {
                two.CopyToGPU();
            }

            kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUMult);
            resultBuffer = accelerator.Allocate<double>(one.GetSize(0), two.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernel(resultBuffer.Extent, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            FastMatrix returnMatrix = new FastMatrix(resultBuffer.GetAs2DArray());
            return returnMatrix;
        }

        public override FastMatrix Subtract(FastMatrix one, FastMatrix two)
        {
            if (one == null || two == null)
            {
                throw new ArgumentNullException();
            }
            if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
            {
                throw new BadDimensionException("The matrices to be added do not have the same sizes!");
            }

            Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
            int groupSize = accelerator.MaxNumThreadsPerGroup;
            SharedMemoryConfig config;
            KernelConfig kernelConfig;
            MemoryBuffer2D<double> resultBuffer;

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
                kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUSub);
                config = SharedMemoryConfig.RequestDynamic<double>(groupSize);
            }
            else
            {
                kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUSubFallback);
                config = SharedMemoryConfig.Empty;
            }

            resultBuffer = accelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));

            one.WaitForCopy();
            two.WaitForCopy();

            kernelConfig = ((resultBuffer.Length + groupSize - 1) / groupSize, groupSize, config);
            kernel(kernelConfig, one.buffer.View, two.buffer.View, resultBuffer.View);

            accelerator.Synchronize();

            FastMatrix returnMatrix = new FastMatrix(resultBuffer.GetAs2DArray());
            return returnMatrix;
        }

        public override FastMatrix Transpose(FastMatrix matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException();
            }
            Accelerator accelerator;
            MemoryBuffer2D<double> resultBuffer;

            if (matrix.buffer == null)
            {
                matrix.CopyToGPU();
            }

            accelerator = HardwareAcceleratorManager.GPUAccelerator;
            resultBuffer = accelerator.Allocate<double>(matrix.GetSize(1), matrix.GetSize(0));
            var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>>(GPUTranspose);
            matrix.WaitForCopy();

            kernel(resultBuffer.Extent, matrix.buffer.View, resultBuffer.View);
            accelerator.Synchronize();
            return new FastMatrix(resultBuffer.GetAs2DArray());
        }

        #region Kernels
        private static void GPUAdd(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<double>().As2DView(group);
            shared[group] = aView[group] + bView[group];
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUAddFallback(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = aView[group] + bView[group];
        }
        private static void GPUSub(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            int globalX = Grid.GlobalIndex.X;
            int globalY = Grid.GlobalIndex.Y;
            Index2 group = new Index2(x, y);
            Index2 global = new Index2(globalX, globalY);
            var shared = SharedMemory.GetDynamic<double>().As2DView(group);
            shared[group] = aView[group] - bView[group];
            Group.Barrier();
            resView[global] = shared[group];
        }
        private static void GPUSubFallback(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
        {
            int x = Group.IdxX;
            int y = Group.IdxY;
            Index2 group = new Index2(x, y);
            resView[group] = aView[group] - bView[group];
        }
        private static void GPUMult(Index2 index, ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
        {
            int x = index.X; //matrix one row
            int y = index.Y; //matrix two column
            double sum = 0;
            for (int i = 0; i < aView.Height; i++)
            {
                sum += aView[x, i] * bView[i, y];
            }
            resView[index] = sum;
        }
        private static void GPUTranspose(Index2 index, ArrayView2D<double> originalView, ArrayView2D<double> result)
        {
            result[index.Y, index.X] = originalView[index.X, index.Y];
        }
        #endregion
    }
}
