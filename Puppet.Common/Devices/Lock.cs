using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void Lock()
        {
            _hub.DoAction(this, "lock");
        }

        public void Unlock()
        {
            _hub.DoAction(this, "unlock");
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
