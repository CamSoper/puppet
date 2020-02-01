using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Devices
{
    public enum MotionStatus
    {
        Active,
        Inactive,
        Unknown
    }
    public class MotionSensor : DeviceBase
    {
        public MotionSensor(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public MotionStatus Status
        {
            get
            {
                switch (GetState()["motion"])
                {
                    case "active":
                        return MotionStatus.Active;

                    case "inactive":
                        return MotionStatus.Inactive;

                    default:
                        return MotionStatus.Unknown;
                }

            }
        }

        public bool IsActive
        {
            get
            {
                return this.Status == MotionStatus.Active;
            }
        }
    }
}
