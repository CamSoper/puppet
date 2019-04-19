using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System.Threading.Tasks;

namespace Puppet.Common.Automation
{
    public abstract class AutomationBase : IAutomation
    {
        protected HomeAutomationPlatform _hub;
        protected HubEvent _evt;
        protected CancellationToken _token;

        public AutomationBase(HomeAutomationPlatform hub, HubEvent evt)
        {
            _hub = hub;
            _evt = evt;
        }

        public Task Handle(CancellationToken token)
        {
            _token = token;
            return Handle();
        }

        protected abstract Task Handle();

        /// <summary>
        /// Waits for a specified period time then returns a boolean indicating if the task was cancelled.
        /// </summary>
        /// <param name="howLong">How long to wait.</param>
        protected async Task WaitForCancellationAsync(TimeSpan howLong) => await Task.Delay(howLong, _token);
   
    }
}
