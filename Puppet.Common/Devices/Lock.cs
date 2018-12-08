using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Devices
{
    public class LockDevice : IDevice
    {
        HomeAutomationPlatform _hub;
        public string Id { get; }

        public string Name => throw new NotImplementedException();

        public string Label => throw new NotImplementedException();

        public LockDevice(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }

        public void Lock()
        {
            _hub.DoAction(this, "lock", null);
        }

        public void Unlock()
        {
            _hub.DoAction(this, "unlock", null);
        }
    }
}
