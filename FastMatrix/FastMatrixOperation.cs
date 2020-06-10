using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using Microsoft.Win32.SafeHandles;

namespace FastMatrixOperations
{
    //TODO: Fix the class structure dumpster fire
    /// <summary>
    /// Contains operation classes.
    /// </summary>
    /// <remarks>
    /// All operation classes contain three functions:
    /// <para>1. Running on a single thread</para>
    /// <para>2. Running on multiple threads</para>
    /// <para>3. Running on the GPU</para>
    /// <para>Note: Running operations parallel or on the GPU is not necessarily faster due to the overhead in copying to memory, thread creation, etc. Pick the function that best suits your case.</para>
    /// </remarks>
    public static class FastMatrixOperation
    {
        /// <summary>
        /// Contains functions that add two matrices
        /// </summary>
        public static class Add
        {
            /// <summary>
            /// Adds two matrices using a single thread on the CPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
            /// <returns>The resulting matrix.</returns>
            public static FastMatrix CPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
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
                Console.WriteLine("CPU took " + stopwatch.ElapsedMilliseconds + "ms");
                return fastMatrix;
            }

            /// <summary>
            /// Adds two matrices using multiple threads on the CPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
            /// <returns>The resulting matrix.</returns>
            /// <remarks>Recommended for matrices larger than 2000 x 2000</remarks>
            public static FastMatrix CPUParallel(FastMatrix one, FastMatrix two)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
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
                Console.WriteLine("Parallel took " + stopwatch.ElapsedMilliseconds + "ms");
                return fastMatrix;
            }

            /// <summary>
            /// Adds two matrices using multiple threads on the GPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
            /// <returns>The resulting matrix.</returns>
            /// <remarks>Not recommended for a single operation due to buffer allocation overhead. 
            /// However, it is extremely fast if the matrices have already been moved to GPU memory.
            /// Runs faster on CUDA due to support for shared memory.
            /// </remarks>
            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch watch2 = Stopwatch.StartNew();

                Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;

                //start tasks
                if (one.buffer == null)
                {
                    one.CopyToGPU();
                }

                if(two.buffer == null)
                {
                    two.CopyToGPU();
                }

                //do as mutch stuff as possible here
                //create kernel
                Stopwatch bruh = Stopwatch.StartNew();
                Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAdd);
                }
                else
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAddFallback);
                }
                Console.WriteLine("Kernel creation took: " + bruh.ElapsedMilliseconds + "ms");

                //config
                int groupSize = accelerator.MaxNumThreadsPerGroup;
                SharedMemoryConfig config;
                KernelConfig kernelConfig;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    config = SharedMemoryConfig.RequestDynamic<double>(groupSize);
                }
                else
                {
                    config = SharedMemoryConfig.Empty;
                }

                var bufferResult = accelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));

                one.WaitForCopy();
                two.WaitForCopy();

                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Reset();
                watch2.Start();

                kernelConfig = ((bufferResult.Length + groupSize - 1) / groupSize, groupSize, config);
                kernel(kernelConfig, one.buffer.View, two.buffer.View, bufferResult.View);

                accelerator.Synchronize();
                Console.WriteLine("Kernel operation " + watch2.ElapsedMilliseconds + "ms");

                Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                FastMatrix returnMatrix = new FastMatrix(bufferResult.GetAs2DArray());
                return returnMatrix;
            }

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
        }


        /// <summary>
        /// Contains functions that subtract matrices.
        /// </summary>
        public static class Subtract
        {
            /// <summary>
            /// Adds two matrices using a single thread on the CPU.
            /// </summary>
            /// <param name="one">A matrix.</param>
            /// <param name="two">The matrix to subtract from the first matrix.</param>
            /// <returns>The resulting matrix.</returns>
            public static FastMatrix CPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
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
                Console.WriteLine("CPU took " + stopwatch.ElapsedMilliseconds + "ms");
                return fastMatrix;
            }

            /// <summary>
            /// Adds two matrices using multiple threads on the CPU.
            /// </summary>
            /// <param name="one">A matrix.</param>
            /// <param name="two">The matrix to subtract from the first matrix.</param>
            /// <returns>The resulting matrix.</returns>
            public static FastMatrix CPUParallel(FastMatrix one, FastMatrix two)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
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
                Console.WriteLine("Parallel took " + stopwatch.ElapsedMilliseconds + "ms");
                return fastMatrix;
            }

            /// <summary>
            /// Subtracts two matrices using multiple threads on the GPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
            /// <returns>The resulting matrix.</returns>
            /// <remarks>Not recommended for a single operation due to buffer allocation overhead. 
            /// However, it is extremely fast if the matrices have already been moved to GPU memory.
            /// Runs faster on CUDA due to support for shared memory.
            /// </remarks>
            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                if((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
                {
                    throw new BadDimensionException("Size are not equal!");
                }

                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch watch2 = Stopwatch.StartNew();

                Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;

                //start tasks
                if (one.buffer == null)
                {
                    one.CopyToGPU();
                }

                if (two.buffer == null)
                {
                    two.CopyToGPU();
                }

                //do as mutch stuff as possible here
                //create kernel

                Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUSub);
                }
                else
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUSubFallback);
                }

                //config
                int groupSize = accelerator.MaxNumThreadsPerGroup;
                SharedMemoryConfig config;
                KernelConfig kernelConfig;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    config = SharedMemoryConfig.RequestDynamic<double>(groupSize);
                }
                else
                {
                    config = SharedMemoryConfig.Empty;
                }

                var bufferResult = accelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));

                one.WaitForCopy();
                two.WaitForCopy();

                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Reset();
                watch2.Start();

                kernelConfig = ((bufferResult.Length + groupSize - 1) / groupSize, groupSize, config);
                kernel(kernelConfig, one.buffer.View, two.buffer.View, bufferResult.View);

                accelerator.Synchronize();
                Console.WriteLine("Kernel operation " + watch2.ElapsedMilliseconds + "ms");

                Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                FastMatrix returnMatrix = new FastMatrix(bufferResult.GetAs2DArray());
                return returnMatrix;
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
        }

        /// <summary>
        /// Contains functions that multiply matrices.
        /// </summary>
        /// <remarks>Keep in mind that matrix multiplication is not always commutative.</remarks>
        public static class Multiply
        {
            //no safety checks, do checks before
            private static double MultiplyArrays(double[] row, double[] column)
            {
                double sum = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    sum += row[i] * column[i];
                }

                return sum;
            }

            /// <summary>
            /// Multiplies two matrices using a single thread on the CPU.
            /// </summary>
            /// <param name="one">First matrix</param>
            /// <param name="two">Second matrix</param>
            /// <returns>The result of the multiplication.</returns>
            public static FastMatrix CPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();

                FastMatrix returnMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));
                if (one.GetSize(1) != two.GetSize(0))
                {
                    throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
                }

                for (int i = 0; i < returnMatrix.GetSize(0); i++)
                {
                    for (int j = 0; j < returnMatrix.GetSize(1); j++)
                    {
                        returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CPU took " + watch.ElapsedMilliseconds + "ms");
                Console.ForegroundColor = ConsoleColor.Green;
                return returnMatrix;
            }

            /// <summary>
            /// Multiplies two matrices using multiple threads on the CPU.
            /// </summary>
            /// <param name="one">First matrix</param>
            /// <param name="two">Second matrix</param>
            /// <returns>The result of the multiplication.</returns>
            public static FastMatrix CPUParallel(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                FastMatrix returnMatrix = new FastMatrix(one.GetSize(0), two.GetSize(1));
                if (one.GetSize(1) != two.GetSize(0))
                {
                    throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
                }

                Parallel.For(0, returnMatrix.GetSize(0), (i) =>
                {
                    for (int j = 0; j < returnMatrix.GetSize(1); j++)
                    {
                        returnMatrix[i, j] = MultiplyArrays(one.GetRow(i), two.GetColumn(j));
                    }
                });
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CPUParallel took " + watch.ElapsedMilliseconds + "ms");
                Console.ForegroundColor = ConsoleColor.Green;
                return returnMatrix;
            }

            /// <summary>
            /// Multiplies two matrices using multiple threads on the GPU.
            /// </summary>
            /// <param name="one">First matrix</param>
            /// <param name="two">Second matrix</param>
            /// <returns>The result of the multiplication.</returns>
            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch watch2 = Stopwatch.StartNew();

                if (one.GetSize(1) != two.GetSize(0))
                {
                    throw new BadDimensionException("Matrices to be multiplied do not have compliant sizes! The number of columns in matrix one is " + one.GetSize(1) + " while the number of rows in matrix two is " + two.GetSize(0));
                }
                Console.WriteLine("Checking size took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Restart();

                Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;
                Console.WriteLine("Getting accelerator took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Restart();

                //start tasks
                if (one.buffer == null)
                {
                    one.CopyToGPU();
                }

                if (two.buffer == null)
                {
                    two.CopyToGPU();
                }
                Console.WriteLine("Starting tasks took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Restart();
                //do as much stuff as possible here
                //create kernel

                var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUMult);
                Console.WriteLine("Kernel creation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Restart();

                var bufferResult = accelerator.Allocate<double>(one.GetSize(0), two.GetSize(1));

                one.WaitForCopy();
                two.WaitForCopy();

                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Restart();

                kernel(bufferResult.Extent, one.buffer.View, two.buffer.View, bufferResult.View);

                accelerator.Synchronize();
                Console.WriteLine("Kernel operation " + watch2.ElapsedMilliseconds + "ms");

                Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                FastMatrix returnMatrix = new FastMatrix(bufferResult.GetAs2DArray());
                return returnMatrix;
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
        }

        public static class Transpose
        {
            public static FastMatrix CPU(FastMatrix matrix)
            {
                FastMatrix returnMatrix = new FastMatrix(matrix.GetSize(1), matrix.GetSize(0));
                for(int i = 0; i < matrix.GetSize(0); i++)
                {
                    for(int j = 0; j< matrix.GetSize(1); j++)
                    {
                        returnMatrix[j, i] = matrix[i, j];
                    }
                }
                return returnMatrix;
            }

            public static FastMatrix CPUParallel(FastMatrix matrix)
            {
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

            public static FastMatrix GPU(FastMatrix matrix)
            {
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

            private static void GPUTranspose(Index2 index, ArrayView2D<double> originalView, ArrayView2D<double> result)
            {
                result[index.Y, index.X] = originalView[index.X, index.Y];
            }
        }
    }
}
