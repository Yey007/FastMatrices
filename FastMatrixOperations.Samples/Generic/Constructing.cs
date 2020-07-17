using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastMatrixOperations.Samples.Generic
{
    static class Constructing
    {
        static void Construct()
        {
            //constructs a 10 row by 3 column matrix
            FastMatrix<IntWrapper> withSize = new FastMatrix<IntWrapper>(10, 3);

            IntWrapper[,] array = new IntWrapper[10, 3];
            //constructs a 10 * 3 matrix from the multidimensional array
            FastMatrix<IntWrapper> with2DArray = new FastMatrix<IntWrapper>(array);

            IntWrapper[][] jaggedArray = new IntWrapper[10][];
            for(IntWrapper i = 0, n = jaggedArray.Length; i < n; i++)
            {
                jaggedArray[i] = new IntWrapper[3];
            }
            //constructs a 10 * 3 matrix from the jagged array
            FastMatrix<IntWrapper> withJaggedArray = new FastMatrix<IntWrapper>(jaggedArray);

            IntWrapper[][] badJaggedArray = new IntWrapper[10][];
            for (IntWrapper i = 0, n = jaggedArray.Length; i < n; i++)
            {
                if (i == 0)
                {
                    badJaggedArray[i] = new IntWrapper[4];
                }
                else
                {
                    badJaggedArray[i] = new IntWrapper[3];
                }
            }
            //throws an exception
            //FastMatrix<int> withBadJaggedArray = new FastMatrix<int>(badJaggedArray);
        }
    }
}
