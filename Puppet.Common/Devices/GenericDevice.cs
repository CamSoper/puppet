using System;
using System.Collections.Generic;
using System.Text;
using Puppet.Common.Services;

namespace Puppet.Common.Devices
{
    public class GenericDevice : DeviceBase
    {
        public GenericDevice(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public void DoAction(string command, string parameter = null)
        {
            string[] args = null;
            if (parameter != null)
            {
                args = new string[] { parameter };
            }
            _hub.DoAction(this, command, args);
        }
    }
}
