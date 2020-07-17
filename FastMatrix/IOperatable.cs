using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastMatrixOperations
{
    public interface IOperatable<T>
    {
        T Add(T t);
        T Subtract(T t);
        T Multiply(T t);
    }

    public interface IGPUOperatable<T> : IOperatable<T>
        where T: unmanaged
    {

    }
}
