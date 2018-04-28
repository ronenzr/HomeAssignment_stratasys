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
    /// <summary>
    /// PrintExecutor - Executes print jobs in a synchronous way using execution queue.
    /// </summary>
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

        /// <summary>
        /// CancelCurrent - Cancels currently executed job. will be remove from queue.
        /// </summary>
        public void CancelCurrent()
        {
            currentJob = null;
            if (this.isWorking)
            {
                timer.Change(0, defaultInterval);
            }
        }

        /// <summary>
        /// Execute - Should actually be doing the printing.
        ///           But for now it is just doing nothing without causing blocking.
        /// </summary>
        /// <param name="toExecute">toExecute - job to execute</param>
        public void Execute(JobDetails toExecute)
        {
            lock (classLock)
            {
                timer.Change(toExecute.Duration, defaultInterval);
                currentJob.Status.SetStatus(PrintStatus.Printing.ToString());
            }
            //TODO: print
        }

        /// <summary>
        /// Start - Starting..
        /// </summary>
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

        /// <summary>
        /// Stop - Stoping..
        /// </summary>
        public void Stop()
        {
            timer.Dispose();
            currentJob = null;
            isWorking = false;
        }
        
    }
}
