
using System;
using System.Diagnostics;

namespace FastMatrixOperations.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Stopwatch watch = Stopwatch.StartNew();

            GPUOperator<DoubleWrapper> gpu = new GPUOperator<DoubleWrapper>();
            Console.WriteLine($"GPU init: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            BufferedFastMatrix<DoubleWrapper> one = new BufferedFastMatrix<DoubleWrapper>(1000, 1000);
            BufferedFastMatrix<DoubleWrapper> two = new BufferedFastMatrix<DoubleWrapper>(1000, 1000);
            Console.WriteLine($"Two 1000x1000 allocations: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            Utilities.FillMatrix(one, random.NextDouble());
            Utilities.FillMatrix(two, random.NextDouble());
            Console.WriteLine($"Filling matrices: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            gpu.Add(one, two);
            Console.WriteLine($"Adding matrices: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            gpu.Multiply(one, two);
            Console.WriteLine($"Multiplying matrices: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
        }
    }
}

