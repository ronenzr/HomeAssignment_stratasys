using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.Common.Entities
{
    public class JobDetails
    {
        public JobDetails()
        {
            Status = new JobStatus(PrintStatus.Queued.ToString(), DateTime.Now);
        }
        public String Name { get; set; }
        public int Duration { get; set; }
        public IStatus Status { get; set; }
    }
}
