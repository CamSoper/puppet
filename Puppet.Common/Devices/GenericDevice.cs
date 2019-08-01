using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Puppet.Common.Services;

namespace Puppet.Common.Devices
{
    public class GenericDevice : DeviceBase
    {
        public GenericDevice(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public async Task DoAction(string command, string parameter = null)
        {
            string[] args = null;
            if (parameter != null)
            {
                args = new string[] { parameter };
            }
            await _hub.DoAction(this, command, args);
        }
    }
}
