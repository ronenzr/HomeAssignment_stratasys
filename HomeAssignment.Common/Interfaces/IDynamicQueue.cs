using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.ApplicationServices.Interfaces
{
    public interface IDynamicQueue<T>
    {
        T Enqueue(T obj);
        T Dequeue();
        T Peek();
        int MoveUp(T obj);
        int MoveDown(T obj);
        bool Remove(T obj);
        IEnumerable<T> ToList();
    }
}
