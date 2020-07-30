using System;

namespace FastMatrixOperations.Internal
{
    public class JaggedArrayException : Exception
    {
        public JaggedArrayException()
        {
        }

        public JaggedArrayException(string message) : base(message)
        {
        }

        public JaggedArrayException(string message, Exception inner) : base(message, inner)
        {
        }

        public JaggedArrayException(int expectedLength, int actualLength, int rowNumber)
            : base($"Array to be converted to matrix is jagged.\n" + 
                  $"Expected {expectedLength} elements but got {actualLength} elements\n" + 
                  $"on row {rowNumber}")
        {

        }
    }
}
