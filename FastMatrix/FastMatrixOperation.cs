using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ILGPU;
using ILGPU.Runtime;

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
    /// <para>Note: Running operations parallel or on the GPU is not necessarily faster due to the overhead in creating threads, etc. Pick the function that best suits the case.</para>
    /// </remarks>
    public class FastMatrixOperation
    {
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
            /// Adds two matrices using multiple threads on the GPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
            /// <returns>The resulting matrix.</returns>
            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch watch = Stopwatch.StartNew();
                using(var context = new Context())
                {
                    using(var accelerator = Accelerator.Create(context, Accelerator.Accelerators[1]))
                    {
                        var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(GPUAdd);

                        using (var aBuffer = accelerator.Allocate<double>(one.GetSize(0), one.GetSize(1)))
                        using (var bBuffer = accelerator.Allocate<double>(two.GetSize(0), two.GetSize(1)))
                        using (var resBuffer = accelerator.Allocate<double>(one.GetSize(0), two.GetSize(1)))
                        {
                            aBuffer.CopyFrom(one.array2d, Index2.Zero, Index2.Zero, aBuffer.Extent);
                            bBuffer.CopyFrom(two.array2d, Index2.Zero, Index2.Zero, bBuffer.Extent);

                            kernel(resBuffer.Extent, aBuffer, bBuffer, resBuffer);
                            accelerator.Synchronize();

                            Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
                            return new FastMatrix(resBuffer.GetAs2DArray());
                        }
                    }
                }
            }

            /// <summary>
            /// Adds two matrices using multiple threads on the CPU.
            /// </summary>
            /// <param name="one">The first matrix to add.</param>
            /// <param name="two">The second matrix to add.</param>
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
                        fastMatrix[i, j] = one[i, j] + two[i, j];
                    }
                });
                Console.WriteLine("Parallel took " + stopwatch.ElapsedMilliseconds + "ms");
                return fastMatrix;
            }

            private static void GPUAdd(Index2 index, ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> resView)
            {
                int x = index.X;
                int y = index.Y;
                resView[index] = aView[new Index2(x, y)] + bView[new Index2(x, y)];
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

            /*
            /// <summary>
            /// Subtract two matrices using multiple threads on the GPU.
            /// </summary>
            /// <param name="one">A matrix.</param>
            /// <param name="two">The matrix to subtract from the first matrix.</param>
            /// <returns>The resulting matrix.</returns>
            public static FastMatrix GPU(FastMatrix one, FastMatrix two)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //check sizes
                if ((one.GetSize(0) != two.GetSize(0)) || (one.GetSize(1) != two.GetSize(1)))
                {
                    throw new BadDimensionException("The matrices to be added do not have the same sizes!");
                }

                //Cudafy setup
                CudafyModule km = CudafyTranslator.Cudafy();
                GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target);
                gpu.LoadModule(km);

                //create return array
                double[,] returnArray = new double[one.GetSize(0), one.GetSize(1)];

                //create array to act as middleman in gpu memory
                double[,] devArray = gpu.Allocate<double>(one.GetSize(0), one.GetSize(1));

                //allocate the matrix arrays on gpu memory
                double[,] Constant2DOne = one.CopyToGPU(gpu);
                double[,] Constant2DTwo = two.CopyToGPU(gpu);

                //lauch add on (number of rows) threads
                gpu.Launch(one.GetSize(0), 1).GPUSub(Constant2DOne, Constant2DTwo, devArray);

                //copy the finished array back to regular memory
                gpu.CopyFromDevice(devArray, returnArray);

                //free all gpu memory
                gpu.FreeAll();
                Console.WriteLine("GPU took " + stopwatch.ElapsedMilliseconds + "ms");
                return new FastMatrix(returnArray);
            }
            */
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

            /*
            [Cudafy]
            private static void GPUSub(GThread thread, double[,] arr, double[,] arr2, double[,] result)
            {
                int x = thread.blockIdx.x;
                int y = 0;

                while (y < arr.GetLength(1))
                {
                    result[x, y] = arr[x, y] - arr2[x, y];
                }
            }
            */
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
        }

    }
}
