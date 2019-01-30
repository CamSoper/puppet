using Puppet.Common.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Automation
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerDeviceAttribute : Attribute
    {
        readonly string _deviceMappedName;
        readonly Capability _capability;

        // This is a positional argument
        public TriggerDeviceAttribute(string deviceMappedName, Capability capability)
        {
            _deviceMappedName = deviceMappedName;
            _capability = capability;
        }

        public string DeviceMappedName
        {
            get { return _deviceMappedName; }
        }

        public Capability Capability
        {
            get { return _capability; }
        }
    }
}
