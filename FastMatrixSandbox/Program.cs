
using System;

namespace FastMatrixOperations.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            GPUOperator<int, IntOperator> gpu = new GPUOperator<int, IntOperator>();
            BufferedFastMatrix<int> one = new BufferedFastMatrix<int>(2, 2);
            BufferedFastMatrix<int> two = new BufferedFastMatrix<int>(2, 2);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    one[i, j] = 10;
                    two[i, j] = 5;
                }
            }

            BufferedFastMatrix<int> result = gpu.Add(one, two);
            result.Print();
            */

            B b = new B();
            b.b = 2;
            A a = (A)b;
            B c = (B)a;
            Console.WriteLine(c.b);
        }
    }

    public abstract class IDC
    {

    }

    public class D : IDC
    {

    }
    
    public class C : IDC
    {

    }

    public abstract class A
    {
        public int a = 5;
        public abstract IDC GetC();
    }

    public class B : A
    {
        public int b = 10;

        public override IDC GetC()
        {
            return new D();
        }
    }
}
