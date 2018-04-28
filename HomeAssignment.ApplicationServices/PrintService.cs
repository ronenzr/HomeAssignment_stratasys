using HomeAssignment.ApplicationServices.Exceptions;
using HomeAssignment.ApplicationServices.Executors;
using HomeAssignment.ApplicationServices.Interfaces;
using HomeAssignment.Common;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace HomeAssignment.ApplicationServices
{
    /// <summary>
    /// PrintService - Provide CRUD operations for printer execution queue.
    ///                Runs printer executor
    /// </summary>
    public class PrintService : IJobService<JobDetails>, IDisposable
    {
        private string backupFileLocation;
        private IDynamicQueue<JobDetails> queue;
        private IJobExecutor<JobDetails> executor;

        public PrintService(string backupFilePath)
        {
            backupFileLocation = backupFilePath;
            LoadQueueFromFile();
            executor = new PrintExecutor(queue);
            executor.Start();
        }

        //for unittests/DI
        public PrintService(IDynamicQueue<JobDetails> _queue, IJobExecutor<JobDetails> _executor)
        {
            queue = _queue;
            executor = _executor;
            executor.Start();
        }

        /// <summary>
        /// AddJob - Adds a new job to the execution queue.
        ///          Jobs with empty names or an existing name will cause exception
        /// </summary>
        /// <param name="jobToAdd">Job to be added to the queue.</param>
        /// <returns>Job Status (Printing/Queued)</returns>
        public IStatus AddJob(JobDetails jobToAdd)
        {
            IStatus status = null;
            JobDetails added = queue.Enqueue(jobToAdd);
            if (added != null)
            {
                status = added.Status;
            }
            return status;
        }

        /// <summary>
        /// DeleteJob - Removes the job from execution queue.
        ///             Trying to remove the top job will throw exception.
        /// </summary>
        /// <param name="job">Job to be removed.</param>
        /// <returns>success/fail</returns>
        public bool DeleteJob(JobDetails job)
        {
            return queue.Remove(job);
        }

        /// <summary>
        /// CancelJob - Cancel the job that is currently being executed.
        ///             Canceled job will also be removed from queue.
        /// </summary>
        /// <returns>success/fail</returns>
        public bool CancelJob()
        {
            try
            {
                executor.CancelCurrent();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// MoveJob - Move job up or down in execution queue, according to direction.
        ///           Trying to move the executed job (top of queue) will throw exception.
        /// </summary>
        /// <param name="job">Job to be moved. (by job name)</param>
        /// <param name="direction">up/down</param>
        /// <returns>Job Status (Printing/Queued)</returns>
        public IStatus MoveJob(JobDetails job, Direction direction)
        {
            int ind = -1;
            if (direction.Equals(Direction.Up))
            {
                ind = queue.MoveUp(job);
            } else
            {
                ind = queue.MoveDown(job);
            }
            
            return GenerateJobStatus(ind);
        }

        /// <summary>
        /// GetJobQueue - Retrieves the execution queue as a collection.
        /// </summary>
        /// <returns>Collection of jobs</returns>
        public IEnumerable<JobDetails> GetJobQueue()
        {
            return queue.ToList();
        }

        /// <summary>
        /// Dispose - Disposing of this service.
        ///           Saving queue to a file and stoping the executor.
        /// </summary>
        public void Dispose()
        {
            Debug.Write("printerService end");
            executor.Stop();
            SaveQueue();
        }

        #region private method
        private IStatus GenerateJobStatus(int ind)
        {
            string status = PrintStatus.Queued.ToString();

            if (ind == 0)
            {
                status = PrintStatus.Printing.ToString();
            }

            return new JobStatus(status, DateTime.Now);
        }


        private void VerifyPrintingStatus(JobDetails job)
        {
            JobDetails printingJob = queue.Peek();
            if (printingJob != null && printingJob.Name.Equals(job.Name))
            {
                throw new OperationOnPrintingJobException();
            }
        }



        private void LoadQueueFromFile()
        {
            IEnumerable<JobDetails> backup = new List<JobDetails>();

            try
            {
                backup = JsonUtility.FromFile<List<JobDetails>>(this.backupFileLocation);
            }
            catch (Exception ex)
            {
            }

            queue = new JobQueue(backup);
        }

        private void SaveQueue()
        {
            IEnumerable<JobDetails> toSave = GetJobQueue();
            JsonUtility.ToFile(backupFileLocation, toSave);
        }
        #endregion
    }
}
