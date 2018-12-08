using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Puppet.Web.SignalR;

namespace Puppet.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventlessAutomationController : ControllerBase
    {
        IHubContext<AutomationHub> _hubContext;

        public EventlessAutomationController(IHubContext<AutomationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET: api/EventlessAutomation
        [HttpGet]
        public ActionResult Get([FromQuery] string automationName)
        {
            _hubContext.Clients.All.SendAsync("handle", automationName, null);
            return new StatusCodeResult(200);
        }

        // POST: api/EventlessAutomation
        [HttpPost]
        public ActionResult Post([FromQuery] string automationName)
        {
            _hubContext.Clients.All.SendAsync("handle", automationName, null);
            return new StatusCodeResult(200);
        }
    }
}
