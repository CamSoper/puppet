
using Puppet.Common.Services;

namespace Puppet.Common.Devices
{
    public class SwitchRelay : DeviceBase
    {
        public SwitchRelay(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public enum SwitchStatus
        {
            On,
            Off,
            Unknown
        }

        public SwitchStatus Status
        {
            get
            {
                // TODO: Go back to the hub to get the current status
                return SwitchStatus.Unknown;
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
