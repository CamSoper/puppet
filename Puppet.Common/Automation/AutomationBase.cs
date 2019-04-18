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

        public AutomationBase(HomeAutomationPlatform hub, HubEvent evt)
        {
            _hub = hub;
            _evt = evt;
            CTS = new CancellationTokenSource();
        }

        public CancellationTokenSource CTS { get; }

        public abstract Task Handle();

        /// <summary>
        /// Waits for a specified period time then returns a boolean indicating if the task was cancelled.
        /// </summary>
        /// <param name="howLong">How long to wait.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>True if the task was cancelled. False if execution should continue.</returns>
        protected async Task<bool> WaitForCancellation(TimeSpan howLong)
        {
            try
            {
                await Task.Delay(howLong, CTS.Token);
                return false;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"{DateTime.Now} Existing {this.GetType()} task cancelled.");
                return true;
            }
        }
    }
}
