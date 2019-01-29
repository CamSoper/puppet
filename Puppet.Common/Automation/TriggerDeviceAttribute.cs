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
        readonly string triggerDeviceId;

        // This is a positional argument
        public TriggerDeviceAttribute(string triggerDeviceId)
        {
            this.triggerDeviceId = triggerDeviceId;

            // TODO: Implement code here

            //throw new NotImplementedException();
        }

        public string TriggerDeviceId
        {
            get { return triggerDeviceId; }
        }
    }
}
