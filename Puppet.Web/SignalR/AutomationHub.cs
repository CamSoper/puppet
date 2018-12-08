using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puppet.Web.SignalR
{
    /// <summary>
    /// This SignalR hub does listens for events and then passes them 
    /// to the executive.
    /// </summary>
    public class AutomationHub : Hub
    {
    }
}
