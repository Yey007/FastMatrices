using System;
using System.Text;
using FastMatrixOperations.Internal;

namespace FastMatrixOperations
{
    public abstract class FastMatrixBase<T>
    {
        public int Rows { get; protected set; }
        public int Columns { get; protected set; }
        public int Length { get; protected set; }

        protected FastMatrixBase(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Length = rows * columns;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                sb.Append("[");
                for (int j = 0; j < Columns; j++)
                {
                    if (j == Columns - 1)
                    {
                        sb.Append(this[i, j]);
                    }
                    else
                    {
                        sb.Append(this[i, j] + ", ");
                    }
                }
                sb.Append("]\n");
            }
            return sb.ToString();
        }
        public T[] GetRow(int row)
        {
            T[] rowData = new T[Columns];
            for (int i = 0; i < rowData.Length; i++)
            {
                rowData[i] = this[row, i];
            }
            return rowData;
        }
        public T[] GetColumn(int column)
        {
            T[] columnData = new T[Rows];
            for (int i = 0; i < columnData.Length; i++)
            {
                columnData[i] = this[i, column];
            }

            return columnData;
        }

        public abstract T this[int row, int column] { get; set; }
        /// <summary>
        /// Override for default equals function
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FastMatrixBase<T>);
        }
        public bool Equals(FastMatrixBase<T> matrix)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(matrix, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, matrix))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != matrix.GetType())
            {
                return false;
            }

            //if sizes aren't same return false
            if ((Rows != matrix.Rows) || (Columns != matrix.Columns))
            {
                return false;
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!matrix[i, j].Equals(this[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public abstract override int GetHashCode();
        public static bool operator ==(FastMatrixBase<T> one, FastMatrixBase<T> two)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(one, null))
            {
                if (Object.ReferenceEquals(two, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            return one.Equals(two);
        }
        public static bool operator !=(FastMatrixBase<T> one, FastMatrixBase<T> two)
        {
            return !(one == two);
        }
    }
}
