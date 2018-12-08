using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Puppet.Common.Events;


using Puppet.Web.SignalR;

namespace Puppet.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomationController : ControllerBase
    {
        IHubContext<AutomationHub> _hubContext;

        public AutomationController(IHubContext<AutomationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // GET: api/Automation
        [HttpGet]
        public ActionResult Get()
        {
            return new StatusCodeResult(404);
        }

        // POST: api/Automation
        [HttpPost]
        public ActionResult Post(HubEvent evt, [FromQuery] string automationName)
        {
            _hubContext.Clients.All.SendAsync("handle", automationName, evt);
            return new StatusCodeResult(200);
        }
    }
}
