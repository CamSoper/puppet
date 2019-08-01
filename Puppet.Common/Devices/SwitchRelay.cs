
using Puppet.Common.Services;
using System.Threading.Tasks;

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

        public async Task On()
        {
            await _hub.DoAction(this, "on");

        }
        public async Task Off()
        {
            await _hub.DoAction(this, "off");
        }
    }
}
