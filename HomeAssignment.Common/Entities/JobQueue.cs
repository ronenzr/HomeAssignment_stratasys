using HomeAssignment.ApplicationServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HomeAssignment.Common.Exceptions;

namespace HomeAssignment.Common.Entities
{
    /// <summary>
    /// JobQueue - Dynamic queue for managing synchronous execution jobs.
    ///            Provide additional up/down functions on top of queue functionality.
    /// </summary>
    public class JobQueue : IDynamicQueue<JobDetails>
    {
        private Object queueLock = new Object();
        private IList<string> queueList;
        private IDictionary<string, JobDetails> jobDetailsMap;

        public JobQueue(IEnumerable<JobDetails> initialQueue)
        {
            queueList = new List<string>();
            jobDetailsMap = new Dictionary<string, JobDetails>();
            initialLoad(initialQueue);
        }

        /// <summary>
        /// Enqueue - Add new item to the end of a queue.
        ///           jobs in queue are unique, will not add same job twice.
        /// </summary>
        /// <param name="obj">Job to be added to the queue.</param>
        /// <returns>Job details of the added job</returns>
        public JobDetails Enqueue(JobDetails obj)
        {
            lock (queueLock)
            {
                if (obj == null || jobDetailsMap.ContainsKey(obj.Name) || string.IsNullOrEmpty(obj.Name))
                {
                    throw new EnqueueFailureException();
                }

                queueList.Add(obj.Name);
                jobDetailsMap.Add(obj.Name, obj);
            }
            
           
            return obj;
        }

        /// <summary>
        /// Dequeue - Remove and return the head of the queue.
        ///           Return null if queue is empty.
        /// </summary>
        /// <returns>Job details of the head</returns>
        public JobDetails Dequeue()
        {
            JobDetails topJob = null;
            string topJobName = queueList.FirstOrDefault();
            
            if (topJobName != null)
            {
                lock (queueLock)
                {
                    queueList.RemoveAt(0);
                    topJob = jobDetailsMap[topJobName];
                    jobDetailsMap.Remove(topJobName);
                }
            }

            return topJob;
        }

        /// <summary>
        /// Peek - Return the head of the queue.
        ///        Return null if queue is empty.
        /// </summary>
        /// <returns>Job details of the head</returns>
        public JobDetails Peek()
        {
            lock (queueLock)
            {
                string topJobName = queueList.FirstOrDefault();
                if (topJobName == null)
                {
                    return null;
                }

                return jobDetailsMap[topJobName];
            }
        }

        /// <summary>
        /// MoveDown - Move job one step down in the queue.
        ///            Trying to move to head will throw exception
        /// </summary>
        /// <param name="obj">Job to be moved down</param>
        /// <returns>new index of the job moved</returns>
        public int MoveDown(JobDetails obj)
        {
            int index = InputValidation(obj);
            if (index == queueList.Count - 1)
            {
                //at the end of the queue anyway.
                return index;
            }

            return move(index + 1, index);
        }

        /// <summary>
        /// MoveUp - Move job one step up in the queue.
        ///          Trying to move to head will throw exception
        /// </summary>
        /// <param name="obj">Job to be moved up</param>
        /// <returns>new index of the job moved</returns>
        public int MoveUp(JobDetails obj)
        {
            int index = InputValidation(obj);
            if (index == 1)
            {
                //at the beginning of the queue anyway.
                return index;
            }

            return move(index - 1, index);
        }

        /// <summary>
        /// Remove - Remove job from queue, Trying to move to head will throw exception
        /// </summary>
        /// <param name="obj">Job to be removed</param>
        /// <returns>success/fail</returns>
        public bool Remove(JobDetails obj)
        {
            lock (queueLock)
            {
                int index = InputValidation(obj);
                queueList.RemoveAt(index);
                jobDetailsMap.Remove(obj.Name);
                return true;
            }
        }

        /// <summary>
        /// ToList - Returns a collection of items in the order of the queue.
        /// </summary>
        /// <returns>Collection of items</returns>
        public IEnumerable<JobDetails> ToList()
        {
            return queueList.Select(j => jobDetailsMap[j]).ToList();
        }

        #region private methods
        private int move(int newInd, int oldInd)
        {
            lock (queueLock)
            {
                string toMove = queueList[oldInd];
                queueList.RemoveAt(oldInd);
                queueList.Insert(newInd, toMove);
            }
            return newInd;
        }

        private void initialLoad(IEnumerable<JobDetails> initialQueue)
        {
            if (initialQueue == null)
            {
                return;
            }
            foreach (var item in initialQueue)
            {
                Enqueue(item);
            }
        }

        private int InputValidation(JobDetails obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.Name))
            {
                throw new JobQueueOperationException();
            }

            int index = queueList.IndexOf(obj.Name);
            if (index <= 0)
            {
                throw new JobQueueOperationException();
            }

            return index;
        }
        #endregion
    }
}
