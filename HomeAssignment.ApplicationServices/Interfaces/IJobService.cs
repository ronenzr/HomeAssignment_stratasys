using HomeAssignment.Common;
using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.ApplicationServices.Interfaces
{
    public interface IJobService<T>
    {
        IStatus AddJob(T jobToAdd);
        IStatus MoveJob(T job, Direction direction);
        bool DeleteJob(T job);
        bool CancelJob();
        IEnumerable<T> GetJobQueue();
    }
}
