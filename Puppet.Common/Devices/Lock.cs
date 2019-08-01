using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public enum LockStatus
    {
        Locked,
        Unlocked,
        Unknown
    }
    public class LockDevice : DeviceBase
    {
        public LockDevice(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public async Task Lock()
        {
            await _hub.DoAction(this, "lock");
        }

        public async Task Unlock()
        {
            await _hub.DoAction(this, "unlock");
        }

        public LockStatus Status
        {
            get
            {
                switch (GetState()["lock"])
                {
                    case "locked":
                        return LockStatus.Locked;

                    case "unlocked":
                        return LockStatus.Unlocked;

                    default:
                        return LockStatus.Unknown;
                }

            }
        }
    }
}
