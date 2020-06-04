using System;
using FastMatrixOperations;

namespace FastMatrixTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int size = 12000;
            Random random = new Random();
            double[][] array = new double[size][];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new double[size];
                for (int j = 0; j < array[i].Length; j++)
                {
                    array[i][j] = random.Next(-1000, 1000);
                }
            }

            FastMatrix bruh = new FastMatrix(array);

            double[][] arr = new double[size][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new double[size];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = random.Next(-1000, 1000);
                }
            }

            FastMatrix help = new FastMatrix(arr);

            //help.Print();
            //Console.WriteLine();
            //bruh.Print();
            //Console.WriteLine();

            FastMatrixOperation.Add.CPU(bruh, help);
            FastMatrixOperation.Add.CPUParallel(bruh, help);
            FastMatrixOperation.Add.GPU(bruh, help);
            Console.ReadLine();
        }
    }
}
