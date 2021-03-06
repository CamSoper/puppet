using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using Puppet.Api.Models;
using Puppet.Executive.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Puppet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubitatEventController : ControllerBase
    {
        IExecutive _executive;

        public HubitatEventController(IExecutive executive)
        {
            _executive = executive;
        }

        // GET: api/<WebhookController>
        [HttpGet]
        public ActionResult Get()
        {
            return new ContentResult() { Content = "☕", StatusCode = 418 };
        }


        // POST api/<WebhookController>
        [HttpPost]
        public void Post([FromBody] HubitatNotification notification)
        {
            _executive.ProcessEvent(notification.Content);
        }
    }
}
