
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
            var threeDPrinter = new SwitchRelay(_hub, DeviceMap.SwitchRelay.ThreeDPrinter);
            threeDPrinter.Off();
        }
    }
}
