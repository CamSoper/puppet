using System;
using Puppet.Common.Devices;

namespace Puppet.Common.Automation
{
    /// <summary>
    /// Indicates that this IAutomation should be triggered by the specified device's capability.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerDeviceAttribute : Attribute
    {
        private readonly string _deviceMappedName;
        private readonly Capability _capability;

        public TriggerDeviceAttribute(string deviceMappedName, Capability capability)
        {
            _deviceMappedName = deviceMappedName;
            _capability = capability;
        }

        public string DeviceMappedName => _deviceMappedName;
        public Capability Capability => _capability;
    }
}
