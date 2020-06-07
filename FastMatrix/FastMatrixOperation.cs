using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;

namespace FastMatrixOperations
{

    /// <summary>
    /// Contains operation classes.
    /// </summary>
    /// <remarks>
    /// All operation classes contain three functions:
    /// <para>1. Running on a single thread</para>
    /// <para>2. Running on multiple threads</para>
    /// <para>3. Running on the GPU</para>
    /// <para>Note: Running operations parallel or on the GPU is not necessarily faster due to the overhead in copying to memory. Pick the function that best suits your case.</para>
    /// </remarks>
    public class FastMatrixOperation
    {
        private static MemoryBuffer2D<double> AllocateBuffer(int sizeX, int sizeY)
        {
            return HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(sizeX, sizeY);
        }

        /// <summary>
        /// Contains functions that add two matrices
        /// </summary>
        public class Add
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

                bool waitOne = false;
                bool waitTwo = false;
                
                //start allocation as soon as possible
                MemoryBuffer2D<double> bufferOne;
                MemoryBuffer2D<double> bufferTwo;
                
                //create tasks
                Action oneAllocAction = new Action(one.CopyToGPU);
                Action twoAllocAction = new Action(two.CopyToGPU);
                Task oneAllocTask = new Task(oneAllocAction);
                Task twoAllocTask = new Task(twoAllocAction);

                //start tasks
                if(one.buffer == null)
                {
                    oneAllocTask.Start();
                    waitOne = true;
                }

                if(two.buffer == null)
                {
                    twoAllocTask.Start();
                    waitTwo = true;
                }

                //do as mutch stuff as possible here
                //create kernel
                Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;
                Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAdd);
                }
                else
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAddFallback);
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

                var bufferResult = HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));

                if (waitOne)
                {
                    oneAllocTask.Wait();
                }
                if(waitTwo)
                {
                    twoAllocTask.Wait();
                }

                bufferOne = one.buffer;
                bufferTwo = two.buffer;
                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Reset();
                watch2.Start();

                kernelConfig = ((bufferResult.Length + groupSize - 1) / groupSize, groupSize, config);
                kernel(kernelConfig, bufferOne.View, bufferTwo.View, bufferResult.View);

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
        public class Subtract
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

            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch watch2 = Stopwatch.StartNew();
                var accelerator = HardwareAcceleratorManager.GPUAccelerator;
                var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUSub);
                MemoryBuffer2D<double> bufferOne;
                MemoryBuffer2D<double> bufferTwo;

                if (one.buffer != null)
                {
                    bufferOne = one.buffer;
                }
                else
                {
                    bufferOne = HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));
                    bufferOne.CopyFrom(one.array2d, Index2.Zero, Index2.Zero, bufferOne.Extent);
                    one.buffer = bufferOne;
                }

                if (two.buffer != null)
                {
                    bufferTwo = two.buffer;
                }
                else
                {
                    bufferTwo = HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(two.GetSize(0), two.GetSize(1));
                    bufferTwo.CopyFrom(two.array2d, Index2.Zero, Index2.Zero, bufferTwo.Extent);
                    two.buffer = bufferTwo;
                }
                var bufferResult = HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(one.GetSize(0), one.GetSize(1));
                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Reset();
                watch2.Start();

                kernel(bufferResult.Extent, bufferOne, bufferTwo, bufferResult);
                accelerator.Synchronize();
                Console.WriteLine("Kernel operation + sync took " + watch2.ElapsedMilliseconds + "ms");

                Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                FastMatrix returnMatrix = new FastMatrix(bufferResult.GetAs2DArray());
                //HardwareAcceleratorManager.Dispose();
                return returnMatrix;
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

            private static void GPUSub(Index2 index, ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
            {
                int x = index.X;
                int y = index.Y;
                resView[index] = aView[new Index2(x, y)] - bView[new Index2(x, y)];
            }
        }

        /// <summary>
        /// Contains functions that multiply matrices.
        /// </summary>
        /// <remarks>Keep in mind that matrix multiplication is not always commutative.</remarks>
        public class Multiply
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
            /// <remarks>Appears to be advantageous for multiplying matrices smaller than 200 x 200</remarks>
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
            /// Multiplies two matrices using a single thread on the CPU.
            /// </summary>
            /// <param name="one">First matrix</param>
            /// <param name="two">Second matrix</param>
            /// <returns>The result of the multiplication.</returns>
            /// <remarks>Appears to be advantageous for multiplying matrices larger than 200 x 200</remarks>
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

            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                Stopwatch watch2 = Stopwatch.StartNew();

                bool waitOne = false;
                bool waitTwo = false;

                //start allocation as soon as possible
                MemoryBuffer2D<double> bufferOne;
                MemoryBuffer2D<double> bufferTwo;

                //create tasks
                Action oneAllocAction = new Action(one.CopyToGPU);
                Action twoAllocAction = new Action(two.CopyToGPU);
                Task oneAllocTask = new Task(oneAllocAction);
                Task twoAllocTask = new Task(twoAllocAction);

                //start tasks
                if (one.buffer == null)
                {
                    oneAllocTask.Start();
                    waitOne = true;
                }

                if (two.buffer == null)
                {
                    twoAllocTask.Start();
                    waitTwo = true;
                }

                //do as mutch stuff as possible here
                //create kernel
                Accelerator accelerator = HardwareAcceleratorManager.GPUAccelerator;
                Action<KernelConfig, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>> kernel;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUMult);
                }
                else
                {
                    kernel = accelerator.LoadStreamKernel<ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUMultFallback);
                }

                //config
                int groupSize = accelerator.MaxNumThreadsPerGroup;
                SharedMemoryConfig memoryConfig;
                KernelConfig kernelConfig;
                if (accelerator.AcceleratorType == AcceleratorType.Cuda)
                {
                    memoryConfig = SharedMemoryConfig.RequestDynamic<double>(groupSize);
                    //config = SharedMemoryConfig.Empty;
                }
                else
                {
                    memoryConfig = SharedMemoryConfig.Empty;
                }

                var bufferResult = HardwareAcceleratorManager.GPUAccelerator.Allocate<double>(one.GetSize(0), two.GetSize(1));

                if (waitOne)
                {
                    oneAllocTask.Wait();
                }
                if (waitTwo)
                {
                    twoAllocTask.Wait();
                }

                bufferOne = one.buffer;
                bufferTwo = two.buffer;
                Console.WriteLine("Buffer allocation took " + watch2.ElapsedMilliseconds + "ms");
                watch2.Reset();
                watch2.Start();

                Console.WriteLine(bufferResult.Length);
                Console.WriteLine(groupSize);
                kernelConfig = ((bufferResult.Length + groupSize - 1) / groupSize, groupSize, memoryConfig);
                kernel(kernelConfig, bufferOne.View, bufferTwo.View, bufferResult.View);

                accelerator.Synchronize();
                Console.WriteLine("Kernel operation " + watch2.ElapsedMilliseconds + "ms");

                Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                FastMatrix returnMatrix = new FastMatrix(bufferResult.GetAs2DArray());
                return returnMatrix;
            }

            private static void GPUMult(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
            {
                //x, y, z defines row first matrix, column second matrix, element
                int x = Group.IdxX;
                int y = Group.IdxY;
                int z = Group.IdxZ;
                int globalX = Group.DimX * Grid.IdxX + x;
                int globalY = Group.DimY * Grid.IdxY + y;
                var shared = SharedMemory.GetDynamic<double>().As2DView(Group.DimX, Group.DimY);
                shared[x, y] += aView[x, z] * bView[z, y];
                Group.Barrier();
                resView[globalX, globalY] = shared[x, y];
            }

            private static void GPUMultFallback(ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
            {


            }
        }

    }
}
