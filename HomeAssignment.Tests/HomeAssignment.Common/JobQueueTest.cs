using System;
using System.Collections.Generic;
using HomeAssignment.ApplicationServices.Interfaces;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeAssignment.Tests
{
    [TestClass]
    public class JobQueueTest
    {
        /* 
         * Covering:
         * Constructor
         * Enqueue
         * Dequeue
         * Peek
         * MoveUp
         * MoveDown
         * Remove
         * ToList
        */

        //Constructor:

        [TestMethod]
        public void Create_AnEmptyInitialQueue_WillNotThrow()
        {
            //
            try
            {
                IDynamicQueue<JobDetails> queue = new JobQueue(null);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception was expected, ex: " + ex.Message);
            }
        }


        [TestMethod]
        public void Create_NonEmptyInitialQueue_WillNotThrow()
        {

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                new JobDetails("TestName", 1000)
            };
            //
            try
            {
                IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception was expected, ex: " + ex.Message);
            }
        }

        [TestMethod]
        public void Create_NonEmptyInitialQueue_ReturnCorrectInitialJob()
        {
            var mockJob = new JobDetails("TestName", 1000);
            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob
            };
            //
           
            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);
            JobDetails topJob = queue.Peek();
            Assert.AreEqual(mockJob, topJob);
        }

        //Enqueue:

        [TestMethod]
        [ExpectedException(typeof(EnqueueFailureException), "Empty job details on enqueue causes exception")]
        public void Enqueue_NullJob_Throws()
        {
            IDynamicQueue<JobDetails> queue = new JobQueue(null);
            JobDetails topJob = queue.Enqueue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(EnqueueFailureException), "2 jobs or more with same name causes exception on enqueue")]
        public void Enqueue_TwoJobsWithSameName_Throws()
        {
            //mock
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName", 2000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.Enqueue(mockJob);
            queue.Enqueue(mockJob2);
        }

        [TestMethod]
        [ExpectedException(typeof(EnqueueFailureException), "job with empty name causes exception on enqueue")]
        public void Enqueue_JobWithEmptyName_Throws()
        {
            //mock
            var mockJob = new JobDetails("", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.Enqueue(mockJob);
        }

        //Dequeue:

        [TestMethod]
        public void Dequeue_EmptyInitialQueue_ReturnNull()
        {

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            JobDetails topJob = queue.Dequeue();
            Assert.IsNull(topJob);
        }

        [TestMethod]
        public void Dequeue_NonEmptyInitialQueue_ReturnCorrectJobsInOrder()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };
            //

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            JobDetails topJob = queue.Dequeue();
            Assert.AreEqual(mockJob, topJob);
            topJob = queue.Dequeue();
            Assert.AreEqual(mockJob2, topJob);
        }

        [TestMethod]
        public void Dequeue_AfterEnqueue_ReturnCorrectJobsInOrder()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            var mockJob3 = new JobDetails("TestName3", 2000);

            //

            IDynamicQueue<JobDetails> queue = new JobQueue(null);
            queue.Enqueue(mockJob);
            queue.Enqueue(mockJob2);
            queue.Enqueue(mockJob3);

            JobDetails topJob = queue.Dequeue();
            Assert.AreEqual(mockJob, topJob);
            topJob = queue.Dequeue();
            Assert.AreEqual(mockJob2, topJob);
            topJob = queue.Dequeue();
            Assert.AreEqual(mockJob3, topJob);
        }

        //Peek:

        [TestMethod]
        public void Peek_EmptyQueue_WillNotThrow()
        {

            try
            {
                IDynamicQueue<JobDetails> queue = new JobQueue(null);
                JobDetails job = queue.Peek();
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception was expected, ex: " + ex.Message);
            }
        }

        [TestMethod]
        public void Peek_EmptyQueue_ReturnNull()
        {
            IDynamicQueue<JobDetails> queue = new JobQueue(null);
            JobDetails job = queue.Peek();

            Assert.IsNull(job);
        }

        [TestMethod]
        public void Peek_NonEmptyQueue_ReturnCorrectJob()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);
            JobDetails job = queue.Peek();

            Assert.AreEqual(mockJob, job);
        }

        [TestMethod]
        public void Peek_NonEmptyQueue_WillNotDequeue()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            JobDetails job = queue.Peek();
            Assert.AreEqual(mockJob, job);
            job = queue.Peek();
            Assert.AreEqual(mockJob, job);
        }

        //MoveUp:

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job details must not be null when MoveUp")]
        public void MoveUp_NullJob_Throws()
        {
            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.MoveUp(null);
        }

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job without name will throw exception when MoveUp")]
        public void MoveUp_JobWithEmptyName_Throws()
        {
            var mockJob = new JobDetails("", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.MoveUp(mockJob);
        }

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "moving job thats not in queue throw exception")]
        public void MoveUp_JobInEmptyQueue_Throws()
        {
            var mockJob = new JobDetails("TestName", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            int index = queue.MoveUp(mockJob);
        }

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "moving first job will throw exception")]
        public void MoveUp_FirstJobInQueue_DoNothing()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            
            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            int index = queue.MoveUp(mockJob);
        }

        [TestMethod]
        public void MoveUp_SecondJobInQueue_DoNothing()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            int index = queue.MoveUp(mockJob2);

            Assert.AreEqual(1, index);
            JobDetails topJob = queue.Peek();
            Assert.AreNotEqual(topJob, mockJob2);
        }

        [TestMethod]
        public void MoveUp_ThirdJobInQueue_MoveToSecond()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            var mockJob3 = new JobDetails("TestName3", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2,
                mockJob3
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            int index = queue.MoveUp(mockJob3);

            Assert.AreEqual(1, index);
            JobDetails topJob = queue.Dequeue();
                       topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob3);
            topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob2);
        }

        //MoveDown:

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job details must not be null when MoveDown")]
        public void MoveDown_NullJob_Throws()
        {
            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.MoveDown(null);
        }

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job without name will throw exception when MoveDown")]
        public void MoveDown_JobWithEmptyName_Throws()
        {
            var mockJob = new JobDetails("", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.MoveDown(mockJob);
        }

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "moving first job will throw exception")]
        public void MoveDown_FirstJobInQueue_DoNothing()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            int index = queue.MoveDown(mockJob);
        }

        [TestMethod]
        public void MoveUp_SecondJobInQueue_MoveToThird()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            var mockJob3 = new JobDetails("TestName3", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2,
                mockJob3
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            int index = queue.MoveDown(mockJob2);

            Assert.AreEqual(2, index);
            JobDetails topJob = queue.Dequeue();
            topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob3);
            topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob2);
        }

        //Remove:

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job details must not be null when MoveDown")]
        public void Remove_NullJob_Throws()
        {
            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.Remove(null);
        }

        //This might not be the correct behavior, 
        //but i assume that removing of elements that are not valid cause exception
        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "job without name will throw exception when Remove")]
        public void Remove_JobWithEmptyName_Throws()
        {
            var mockJob = new JobDetails("", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            queue.Remove(mockJob);
        }

        //This might not be the correct behavior, 
        //but i assume that removing of elements not in the queue will cause exception
        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "removing job thats not in queue throw exception")]
        public void Remove_JobInEmptyQueue_Throws()
        {
            var mockJob = new JobDetails("TestName", 1000);

            IDynamicQueue<JobDetails> queue = new JobQueue(null);

            int index = queue.MoveUp(mockJob);
        }

        [TestMethod]
        public void Remove_SecondJobInQueue_RemoveFromQueue()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            var mockJob3 = new JobDetails("TestName3", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2,
                mockJob3
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            bool success = queue.Remove(mockJob2);

            Assert.IsTrue(success);
            JobDetails topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob);
            topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob3);
        }

        [TestMethod]
        public void Remove_ThirdJobInQueue_RemoveFromQueue()
        {
            var mockJob = new JobDetails("TestName", 1000);
            var mockJob2 = new JobDetails("TestName2", 2000);
            var mockJob3 = new JobDetails("TestName3", 2000);

            IEnumerable<JobDetails> initialQueue = new List<JobDetails>()
            {
                mockJob,
                mockJob2,
                mockJob3
            };

            IDynamicQueue<JobDetails> queue = new JobQueue(initialQueue);

            bool success = queue.Remove(mockJob3);

            Assert.IsTrue(success);
            JobDetails topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob);
            topJob = queue.Dequeue();
            Assert.AreEqual(topJob, mockJob2);
            topJob = queue.Peek();
            Assert.IsNull(topJob);
        }


        //ToList:

        [TestMethod]
        public void ToList_EmptyQueue_WillNotThrow()
        {
            try
            {
                IDynamicQueue<JobDetails> queue = new JobQueue(null);

                IEnumerable<JobDetails> list = queue.ToList();
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception was expected, ex: " + ex.Message);
            }
        }
    }
}
