using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    enum ContactStatus
    {
        Open,
        Closed
    }

    public class ContactSensor : IDevice
    {
        ContactSensor(string deviceId, HomeAutomationPlatform hub)
        {

        }

        public string Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string Label => throw new NotImplementedException();
    }
}
