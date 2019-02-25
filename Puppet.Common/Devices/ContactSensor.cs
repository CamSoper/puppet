using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public enum ContactStatus
    {
        Open,
        Closed,
        Unknown
    }

    public class ContactSensor : DeviceBase
    {
        public ContactSensor(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public ContactStatus Status
        {
            get
            {
                switch(GetState()["contact"])
                {
                    case "open":
                        return ContactStatus.Open;

                    case "closed":
                        return ContactStatus.Closed;

                    default:
                        return ContactStatus.Unknown;                  
                }

            }
        }
    }
}
