using HomeAssignment.ApplicationServices;
using HomeAssignment.Common;
using HomeAssignment.Common.Entities;
using HomeAssignment.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeAssignment_Print.Controllers
{
    public class PrinterController : ApiController
    {
        private PrintService printService;

        public PrinterController()
        {
            printService = (PrintService)HttpContext.Current.Application["PrintService"];
        }

        // GET api/printqueue
        [HttpGet()]
        [Route("api/printer/query")]
        public IEnumerable<JobDetails> GetJobList()
        {
            return printService.GetJobQueue();
        }

        [HttpPost()]
        [Route("api/printer")]
        public IStatus Post([FromBody]JobDetails jobToAdd)
        {
            return printService.AddJob(jobToAdd);
        }

        [HttpPut()]
        [Route("api/printer/direction/{direction}")]
        public IStatus Move([FromBody]JobDetails jobToMove, Direction direction)
        {
            return printService.MoveJob(jobToMove, direction);
        }

        [HttpGet()]
        [Route("api/printer/cancel")]
        public bool CancelPrint()
        {
            return printService.CancelJob();
        }

        [HttpDelete()]
        [Route("api/printer")]
        public bool Delete([FromBody]JobDetails jobToMove)
        {
            return printService.DeleteJob(jobToMove);
        }
    }
}
