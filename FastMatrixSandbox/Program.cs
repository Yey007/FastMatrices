
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

            SingleThreadedOperator<DoubleWrapper> cpu = new SingleThreadedOperator<DoubleWrapper>();
            Console.WriteLine($"CPU init: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            MultiThreadedOperator<DoubleWrapper> cpumult = new MultiThreadedOperator<DoubleWrapper>();
            Console.WriteLine($"Parallel init: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            BufferedFastMatrix<DoubleWrapper> one = new BufferedFastMatrix<DoubleWrapper>(64, 64);
            BufferedFastMatrix<DoubleWrapper> two = new BufferedFastMatrix<DoubleWrapper>(64, 64);
            Console.WriteLine($"Two 100x100 allocations: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            one.CopyToGPU();
            two.CopyToGPU();
            Console.WriteLine($"Copy: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            Utilities.FillMatrix(one, 10);
            Utilities.FillMatrix(two, 20);
            Console.WriteLine($"Filling matrices: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            var bruh = gpu.AddShared(one, two);
            Console.WriteLine($"GPU Add: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            gpu.Multiply(one, two);
            Console.WriteLine($"GPU Mult: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            cpu.Add(one, two);
            Console.WriteLine($"CPU Add: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            cpu.Multiply(one, two);
            Console.WriteLine($"CPU Mult: {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            cpumult.Add(one, two);
            Console.WriteLine($"Parallel Add: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            cpumult.Multiply(one, two);
            Console.WriteLine($"Parallel Mult: {watch.ElapsedMilliseconds}ms");
            watch.Restart();
        }
    }
}

