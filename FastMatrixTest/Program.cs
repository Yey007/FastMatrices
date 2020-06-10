using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FastMatrixOperations;

namespace FastMatrixTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            CPUOperator cpu = new CPUOperator();
            ParallelOperator parallel = new ParallelOperator();
            GPUOperator gpu = new GPUOperator();

            int size = 500;
            int size2 = 500;
            Random random = new Random();
            double[][] array = new double[size][];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new double[size2];
                for (int j = 0; j < array[i].Length; j++)
                {
                    array[i][j] = random.Next(-10, 10);
                }
            }

            FastMatrix bruh = new FastMatrix(array);

            double[][] arr = new double[size2][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new double[size];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = random.Next(-10, 10);
                }
            }

            FastMatrix help = new FastMatrix(arr);

            watch.Start();

            ////////////
            //multiply//
            ////////////
            Console.WriteLine("Multiplying");
            var multRes1 = cpu.Multiply(bruh, help);
            Console.WriteLine("CPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var multRes2 = parallel.Multiply(bruh, help);
            Console.WriteLine("Parallel took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var multRes3 = gpu.Multiply(bruh, help);
            Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");

            Debug.Assert(multRes1 == multRes2);
            Debug.Assert(multRes3 == multRes2);
            Console.WriteLine("Enter for next phase");
            Console.ReadLine();

            ////////
            //add//
            ///////
            Console.WriteLine("Adding");
            watch.Restart();
            var addRes1 = cpu.Add(bruh, help);
            Console.WriteLine("CPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var addRes2 = parallel.Add(bruh, help);
            Console.WriteLine("Parallel took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var addRes3 = gpu.Add(bruh, help);
            Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            Debug.Assert(multRes1 == multRes2);
            Debug.Assert(multRes3 == multRes2);
            Console.WriteLine("Enter for next phase");
            Console.ReadLine();

            ////////////
            //subtract//
            ////////////
            Console.WriteLine("Subtracting");
            watch.Restart();
            var subRes1 = cpu.Subtract(bruh, help);
            Console.WriteLine("CPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var subRes2 = parallel.Subtract(bruh, help);
            Console.WriteLine("Parallel took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var subRes3 = gpu.Subtract(bruh, help);
            Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            Debug.Assert(multRes1 == multRes2);
            Debug.Assert(multRes3 == multRes2);
            Console.WriteLine("Enter for next phase");
            Console.ReadLine();

            /////////////
            //transpose//
            /////////////
            Console.WriteLine("Transposing");
            watch.Restart();
            var transRes1 = cpu.Transpose(bruh);
            Console.WriteLine("CPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var transRes2 = parallel.Transpose(bruh);
            Console.WriteLine("Parallel took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            var transRes3 = gpu.Transpose(bruh);
            Console.WriteLine("GPU took " + watch.ElapsedMilliseconds + "ms");
            watch.Restart();

            Debug.Assert(multRes1 == multRes2);
            Debug.Assert(multRes3 == multRes2);
            Console.WriteLine("Enter to finish");
            Console.ReadLine();
            return;
        }
    }
}
