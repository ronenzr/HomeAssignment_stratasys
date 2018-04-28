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

        public JobDetails(string name, int duration)
        {
            Name = name;
            Duration = duration;
            Status = new JobStatus(PrintStatus.Queued.ToString(), DateTime.Now);
        }


        public string Name { get; set; }
        public int Duration { get; set; }
        public IStatus Status { get; set; }
    }
}
