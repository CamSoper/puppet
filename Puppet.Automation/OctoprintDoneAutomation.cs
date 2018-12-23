
using Puppet.Common.Devices;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Puppet.Common.Models.Automation;
using Puppet.Common.Events;

namespace Puppet.Automation
{
    /// <summary>
    /// Turns off 3-D printer after Octoprint finishes printing.
    /// </summary>
    public class OctoprintDoneAutomation : IAutomation
    {
        HomeAutomationPlatform _hub;
        public OctoprintDoneAutomation(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }

        public void Handle(HubEvent evt, CancellationToken token)
        {
            // Turn off printer and light
            var printer = new SwitchRelay(_hub, DeviceMap.SwitchRelay.Printer3D);
            printer.Off();
        }
    }
}
