using HomeAssignment.ApplicationServices.Exceptions;
using HomeAssignment.ApplicationServices.Executors;
using HomeAssignment.ApplicationServices.Interfaces;
using HomeAssignment.Common;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HomeAssignment.ApplicationServices
{
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

        public IStatus AddJob(JobDetails jobToAdd)
        {
            JobDetails added = queue.Enqueue(jobToAdd);
            return added.Status;
        }

        public IStatus MoveJob(JobDetails job, Direction direction)
        {
            VerifyPrintingStatus(job);
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


        public bool DeleteJob(JobDetails job)
        {
            VerifyPrintingStatus(job);
            return queue.Remove(job);
        }

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

        public IEnumerable<JobDetails> GetJobQueue()
        {
            return queue.ToList();
        }

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
            if(printingJob != null && printingJob.Name.Equals(job.Name))
            {
                throw new OperationOnPrintingJobException();
            }
        }

        public void Dispose()
        {
            executor.Stop();
            SaveQueue();
        }

        private void LoadQueueFromFile()
        {
            IEnumerable<JobDetails> backup = new List<JobDetails>();

            try
            {
                backup = JsonUtility.FromFile<List<JobDetails>>(this.backupFileLocation);
            }
            catch (Exception ex) {
            }
                
            queue = new JobQueue(backup);
        }

        private void SaveQueue()
        {
            IEnumerable<JobDetails> toSave = GetJobQueue();
            JsonUtility.ToFile(backupFileLocation, toSave);
        }
    }
}
