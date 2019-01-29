using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Devices;

namespace Puppet.Common.Services
{
    class SmartThings : HomeAutomationPlatform
    {
        public override void DoAction(IDevice device, string action, string[] args = null)
        {
            throw new NotImplementedException();
        }

        public override IDevice GetDevice()
        {
            throw new NotImplementedException();
        }
    }
}
