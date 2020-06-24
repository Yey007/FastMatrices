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
            FastMatrix<int> withSize = new FastMatrix<int>(10, 3);

            int[,] array = new int[10, 3];
            //constructs a 10 * 3 matrix from the multidimensional array
            FastMatrix<int> with2DArray = new FastMatrix<int>(array);

            int[][] jaggedArray = new int[10][];
            for(int i = 0, n = jaggedArray.Length; i < n; i++)
            {
                jaggedArray[i] = new int[3];
            }
            //constructs a 10 * 3 matrix from the jagged array
            FastMatrix<int> withJaggedArray = new FastMatrix<int>(jaggedArray);

            int[][] badJaggedArray = new int[10][];
            for (int i = 0, n = jaggedArray.Length; i < n; i++)
            {
                if (i == 0)
                {
                    badJaggedArray[i] = new int[4];
                }
                else
                {
                    badJaggedArray[i] = new int[3];
                }
            }
            //throws an exception
            //FastMatrix<int> withBadJaggedArray = new FastMatrix<int>(badJaggedArray);
        }
    }
}
