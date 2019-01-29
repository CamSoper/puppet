using Puppet.Common.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Automation
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerDeviceAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string _triggerDeviceId;
        readonly Capability _capability;

        // This is a positional argument
        public TriggerDeviceAttribute(string triggerDeviceId, Capability capability)
        {
            _triggerDeviceId = triggerDeviceId;
            _capability = capability;
        }

        public string TriggerDeviceId
        {
            get { return _triggerDeviceId; }
        }

        public Capability Capability
        {
            get { return _capability; }
        }
    }
}
