using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssignment.Common.Entities
{
    public class JobStatus : IStatus
    {
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }

        public JobStatus(string status, DateTime creationDate)
        {
            this.Status = status;
            this.StatusDate = creationDate;
        }

        public string GetStatus()
        {
            return Status;
        }

        public string SetStatus(string status)
        {
            Status = status;
            return Status;
        }
    }
}
