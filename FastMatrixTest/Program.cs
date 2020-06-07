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
                    array[i][j] = 5;
                }
            }

            FastMatrix bruh = new FastMatrix(array);
            //Action copy1 = bruh.CopyToGPU;
            //Task t1 = new Task(copy1);
            //t1.Start();

            double[][] arr = new double[size2][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new double[size];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = 5;
                }
            }

            FastMatrix help = new FastMatrix(arr);
            //Action copy2 = help.CopyToGPU;
            //Task t2 = new Task(copy2);
            //t2.Start();

            //t1.Wait();
            //t2.Wait();
            //bruh.CopyToGPU();
            //help.CopyToGPU();

            FastMatrix check1 = FastMatrixOperation.Multiply.CPU(bruh, help);
            check1.Print();
            //FastMatrixOperation.Add.CPUParallel(bruh, help).Print();
            FastMatrix check2 = FastMatrixOperation.Multiply.GPU(bruh, help);
            check2.Print();
            Console.WriteLine(check1 == check2);

            Console.WriteLine("Runtime was: " + watch.ElapsedMilliseconds);
            Console.ReadLine();
            FastMatrixOperation.Add.CPU(bruh, help);
            FastMatrixOperation.Add.CPUParallel(bruh, help);
            FastMatrixOperation.Add.GPU(bruh, help);
            Console.ReadLine();
        }
    }
}
