using HomeAssignment.ApplicationServices.Interfaces;
using HomeAssignment.Common;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HomeAssignment.ApplicationServices.Executors
{
    public class PrintExecutor : IJobExecutor<JobDetails>
    {
        private IDynamicQueue<JobDetails> queueList;
        private Timer timer;
        private int defaultInterval = 2000;
        private JobDetails currentJob = null;
        private bool isWorking = false;
        private object classLock = new object();

        public PrintExecutor(IDynamicQueue<JobDetails> _queue)
        {
            queueList = _queue;
        }

        public void CancelCurrent()
        {
            currentJob = null;
            if (this.isWorking)
            {
                timer.Change(0, defaultInterval);
            }
        }

        public void Execute(JobDetails toExecute)
        {
            lock (classLock)
            {
                timer.Change(toExecute.Duration, defaultInterval);
                currentJob.Status.SetStatus(PrintStatus.Printing.ToString());
            }
            //TODO: print
        }

        public void Start()
        {
            isWorking = true;
            timer = new Timer(this.Tick, null, 0, defaultInterval);
        }

        public void Tick(object info)
        {
            lock (classLock)
            {
                if (currentJob != null)
                {
                    //execution was completed. removing task from queue
                    queueList.Dequeue();
                }

                currentJob = queueList.Peek();
                if (currentJob != null) {
                    //queue is not empty, executing job
                    Execute(currentJob);
                }
            }
        }

        public void Stop()
        {
            timer.Dispose();
            currentJob = null;
            isWorking = false;
        }
        
    }
}
