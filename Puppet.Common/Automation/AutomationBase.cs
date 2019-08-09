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
using Puppet.Common.Models;

namespace Puppet.Common.Automation
{
    public abstract class AutomationBase : IAutomation
    {
        protected HomeAutomationPlatform _hub;
        protected HubEvent _evt;
        protected CancellationToken _cancelToken;

        public AutomationBase(HomeAutomationPlatform hub, HubEvent evt)
        {
            _hub = hub;
            _evt = evt;
        }

        public async Task Handle(CancellationToken token)
        {
            _cancelToken = token;
            await InitDevices();
            await Handle();
        }

        protected abstract Task InitDevices();

        protected abstract Task Handle();
        

        /// <summary>
        /// Waits for a specified period time then returns a boolean indicating if the task was cancelled.
        /// </summary>
        /// <param name="howLong">How long to wait.</param>
        protected async Task WaitForCancellationAsync(TimeSpan howLong) => await Task.Delay(howLong, _cancelToken);
   
        /// <summary>
        /// Returns a bool indicating if it's currently dark outside based on sunrise and sunset times.
        /// </summary>
        /// <param name="sunriseOffset">Sunrise offset in minutes.</param>
        /// <param name="sunsetOffset">Sunset offset in minutes.</param>
        protected async Task<bool> IsDark(int sunriseOffset = 0, int sunsetOffset = 0)
        {
            SunriseAndSunset sun = await _hub.GetSunriseAndSunset();
            DateTime SunriseWithOffset = sun.Sunrise.AddMinutes(sunriseOffset);
            DateTime SunsetWithOffset = sun.Sunset.AddMinutes(sunsetOffset);

            if (DateTime.Now >= SunriseWithOffset && 
                DateTime.Now <= SunsetWithOffset)
            {
                return false;   
            }
            else
            {
                return true;
            }
        }
    }
}
