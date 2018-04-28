using System;
using System.Collections.Generic;
using HomeAssignment.ApplicationServices;
using HomeAssignment.ApplicationServices.Interfaces;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Exceptions;
using HomeAssignment.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HomeAssignment.Tests
{
    [TestClass]
    public class PrintServiceTest
    {
        /* 
         * Covering:
         * AddJob
         * DeleteJob
         * CancelJob
         * MoveJob
        */

        private Mock<IDynamicQueue<JobDetails>> mockQueue;
        private Mock<IJobExecutor<JobDetails>> mockExecutor;
        private IJobService<JobDetails> printService;

        [TestInitialize()]
        public void Init()
        {
            mockQueue = new Mock<IDynamicQueue<JobDetails>>();
            mockExecutor = new Mock<IJobExecutor<JobDetails>>();

            //ignore start function
            mockExecutor.Setup(mock => mock.Start()).Verifiable();


            printService = new PrintService(mockQueue.Object, mockExecutor.Object);
        }

        //AddJob:

        [TestMethod]
        [ExpectedException(typeof(EnqueueFailureException), "null job causes exception, expect not to be absorbed")]
        public void AddJob_NullJobDetails_Throws()
        {
            mockQueue.Setup(mock => mock.Enqueue(It.IsAny<JobDetails>())).Throws(new EnqueueFailureException());

            printService.AddJob(null);
        }

        [TestMethod]
        public void AddJob_EnqueueReturnsNull_WillNotThrow()
        {
            var job = new JobDetails("TestJob", 10000);
            mockQueue.Setup(mock => mock.Enqueue(It.IsAny<JobDetails>()))
                     .Returns((JobDetails mock) => { return null; });

            try
            {
                printService.AddJob(job);
            }
            catch (Exception ex)
            {
                Assert.Fail("No exception was expected, ex: " + ex.Message);
            }
        }

        [TestMethod]
        public void AddJob_ValidJobDetails_CallsEnqueueAndReturnStatus()
        {
            var job = new JobDetails("TestJob", 10000);
            mockQueue.Setup(mock => mock.Enqueue(job)).Verifiable();

            printService.AddJob(job);

            mockQueue.Verify(mock => mock.Enqueue(It.IsAny<JobDetails>()), Times.Once());
        }

        //DeleteJob:

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "null job causes exception, expect not to be absorbed")]
        public void DeleteJob_NullJobDetails_Throws()
        {
            mockQueue.Setup(mock => mock.Remove(It.IsAny<JobDetails>())).Throws(new JobQueueOperationException());

            printService.DeleteJob(null);
        }

        [TestMethod]
        public void DeleteJob_ValidJobDetails_CallsRemoveAndReturnObject()
        {
            var job = new JobDetails("TestJob", 10000);
            mockQueue.Setup(mock => mock.Remove(job)).Verifiable();

            printService.DeleteJob(job);

            mockQueue.Verify(mock => mock.Remove(It.IsAny<JobDetails>()), Times.Once());
        }

        //CancelJob:
        
        [TestMethod]
        public void CancelJob_ValidCall_CallsExecutorCancelCurrent()
        {
            mockExecutor.Setup(mock => mock.CancelCurrent()).Verifiable();

            printService.CancelJob();

            mockExecutor.Verify(mock => mock.CancelCurrent(), Times.Once());
        }

        [TestMethod]
        public void CancelJob_ExecutorExceptionThrown_ReturnFalse()
        {
            mockExecutor.Setup(mock => mock.CancelCurrent()).Throws(new Exception());

            var success = printService.CancelJob();

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void CancelJob_NoExecutorExceptionThrown_ReturnTrue()
        {
            mockExecutor.Setup(mock => mock.CancelCurrent()).Verifiable();

            var success = printService.CancelJob();

            Assert.IsTrue(success);
        }

        //MoveJob:

        [TestMethod]
        [ExpectedException(typeof(JobQueueOperationException), "exception thrown, expect not to be absorbed")]
        public void MoveJob_NullJobDetails_Throws()
        {
            mockQueue.Setup(mock => mock.MoveUp(It.IsAny<JobDetails>())).Throws(new JobQueueOperationException());
            mockQueue.Setup(mock => mock.MoveDown(It.IsAny<JobDetails>())).Throws(new JobQueueOperationException());

            printService.MoveJob(null, Common.Direction.Up);
        }

        [TestMethod]
        public void MoveJob_DirectionUp_CallsQueueMoveUp()
        {
            var job = new JobDetails("TestJob", 10000);
            mockQueue.Setup(mock => mock.MoveUp(job)).Verifiable();

            printService.MoveJob(job, Common.Direction.Up);

            mockQueue.Verify(mock => mock.MoveUp(job), Times.Once());
        }

        [TestMethod]
        public void MoveJob_DirectionDown_CallsQueueMoveDown()
        {
            var job = new JobDetails("TestJob", 10000);
            mockQueue.Setup(mock => mock.MoveDown(job)).Verifiable();

            printService.MoveJob(job, Common.Direction.Down);

            mockQueue.Verify(mock => mock.MoveDown(job), Times.Once());
        }
    }
}
