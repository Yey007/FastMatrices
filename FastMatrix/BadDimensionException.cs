using System;

namespace FastMatrixOperations.Internal
{
    /// <summary>
    /// Thrown when either
    /// <para>1. The array that is supposed to be converted to a FastMatrix is jagged</para>
    /// <para>2. An operation is attempting to be done on non-compliant matrices (ex. adding a 3x2 matrix to a 4x2 matrix)</para>
    /// </summary>
    public class BadDimensionException : Exception
    {
        public BadDimensionException()
        {
        }

        public BadDimensionException(string message)
            : base(message)
        {
        }

        public BadDimensionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}