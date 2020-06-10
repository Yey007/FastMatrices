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
            Stopwatch watch = Stopwatch.StartNew();

            int size = 5;
            int size2 = 5;
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

            //bruh.CopyToGPU();
            //help.CopyToGPU();

            //bruh.WaitForCopy();
            //help.WaitForCopy();
            bruh.Print();
            Console.WriteLine();
            
            FastMatrixOperation.Transpose.CPUParallel(bruh).Print();
            Console.WriteLine();
            FastMatrixOperation.Transpose.GPU(bruh).Print();

            Console.WriteLine("Runtime was: " + watch.ElapsedMilliseconds + "ms");
            Console.ReadLine();

            //FastMatrixOperation.Multiply.CPU(bruh, help);
            //FastMatrixOperation.Multiply.CPUParallel(bruh, help);
            //FastMatrixOperation.Multiply.GPU(bruh, help);
            Console.ReadLine();
        }
    }
}
