
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public class SwitchRelay : IDevice
    {
        HomeAutomationPlatform _hub;
        public enum SwitchStatus
        {
            On,
            Off,
            Unknown
        }

        public SwitchStatus Status
        {
            get
            {
                // TODO: Go back to the hub to get the current status
                return SwitchStatus.Unknown;
            }
        }

        public string Id { get; }


        // TODO: Go get these from the hub
        public string Name => throw new NotImplementedException();
        public string Label => throw new NotImplementedException();

        public SwitchRelay(HomeAutomationPlatform hub, string id)
        {
            _hub = hub;
            this.Id = id;
        }

        public void On()
        {
            _hub.DoAction(this, "on");

        }
        public void Off()
        {
            _hub.DoAction(this, "off");
        }
    }
}
