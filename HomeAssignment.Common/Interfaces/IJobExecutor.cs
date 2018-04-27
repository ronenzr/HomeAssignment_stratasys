using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.Common.Interfaces
{
    public interface IJobExecutor<T>
    {
        void Start();
        void Stop();
        void Execute(T toExecute);
        void CancelCurrent();
    }
}
