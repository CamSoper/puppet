using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Devices
{
    public abstract class DeviceBase : IDevice
    {
        internal HomeAutomationPlatform _hub;

        public string Id { get; }
        
        // TODO: Go get these from the hub
        public string Name => throw new NotImplementedException();
        public string Label => throw new NotImplementedException();

        public DeviceBase(HomeAutomationPlatform hub, string id)
        {
            _hub = hub;
            this.Id = id;
        }
    }
}
