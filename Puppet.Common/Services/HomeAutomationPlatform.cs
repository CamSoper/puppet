using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Devices;

namespace Puppet.Common.Services
{
    public abstract class HomeAutomationPlatform
    {
        public ConcurrentDictionary<string, object> StateBag { get; set; }
        public abstract void DoAction(IDevice device, string action, string[] args = null);
        public abstract IDevice GetDevice();
    }
}
