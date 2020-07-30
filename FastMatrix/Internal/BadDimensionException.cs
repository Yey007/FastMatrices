using System;

namespace FastMatrixOperations.Internal
{
    /// <summary>
    /// Thrown when an operation is attempting to be done on non-compliant matrices 
    /// (ex. adding a 3x2 matrix to a 4x2 matrix)</para>
    /// </summary>
    public class BadDimensionException : Exception
    {
        public BadDimensionException()
        {
        }

        public BadDimensionException(string message) : base(message)
        {
        }

        public BadDimensionException(string message, Exception inner) : base(message, inner)
        {
        }

        public BadDimensionException(int expectedRows, int expectedColumns,
            int actualRows, int actualColumns) : base("Wrong dimensions in matrix operation\n" +
                $"Expected {expectedRows} rows and {expectedColumns} columns.\n" +
                $"Got {actualRows} rows and {actualColumns} columns.")
        {

        }
    }
}