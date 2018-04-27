using HomeAssignment.ApplicationServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HomeAssignment.Common.Exceptions;

namespace HomeAssignment.Common.Entities
{
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

        public JobDetails Enqueue(JobDetails obj)
        {
            lock (queueLock)
            {
                if (jobDetailsMap.ContainsKey(obj.Name))
                {
                    throw new EnqueueFailureException();
                }

                queueList.Add(obj.Name);
                jobDetailsMap.Add(obj.Name, obj);
            }
            
           
            return obj;
        }

        public JobDetails Peek()
        {
            if(queueList.FirstOrDefault() == null)
            {
                return null;
            }

            return jobDetailsMap[queueList.First()];
        }

        public int MoveDown(JobDetails obj)
        {
            int index = queueList.IndexOf(obj.Name);
            if(index == queueList.Count - 1)
            {
                //at the end of the queue anyway.
                return index;
            }

            return move(index + 1, index);
        }

        public int MoveUp(JobDetails obj)
        {
            int index = queueList.IndexOf(obj.Name);
            if (index <= 1)
            {
                //at the beginning of the queue anyway.
                return index;
            }

            return move(index - 1, index);
        }

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

        public bool Remove(JobDetails obj)
        {
            if (jobDetailsMap.ContainsKey(obj.Name))
            {
                lock (queueLock)
                {
                    queueList.Remove(obj.Name);
                    jobDetailsMap.Remove(obj.Name);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<JobDetails> ToList()
        {
            return queueList.Select(j => jobDetailsMap[j]).ToList();
        }

        private void initialLoad(IEnumerable<JobDetails> initialQueue)
        {
            if(initialQueue == null)
            {
                return;
            }
            foreach (var item in initialQueue)
            {
                Enqueue(item);
            }
        }
    }
}
