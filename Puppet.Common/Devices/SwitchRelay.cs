
using Puppet.Common.Services;

namespace Puppet.Common.Devices
{
    public enum SwitchStatus
    {
        On,
        Off,
        Unknown
    }
    public class SwitchRelay : DeviceBase
    {

        public SwitchRelay(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public SwitchStatus Status
        {
            get
            {
                switch (GetState()["switch"])
                {
                    case "on":
                        return SwitchStatus.On;

                    case "off":
                        return SwitchStatus.Off;

                    default:
                        return SwitchStatus.Unknown;
                }

            }
        }

        public void On()
        {
            _hub.DoAction(this, "on");

        }
        public void Off()
        {
            _hub.DoAction(this, "off");
        }
    }
}
