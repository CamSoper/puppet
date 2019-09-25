using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Demo.Remote", Capability.Pushed)]
    public class GarageDoorRemoteControl : AutomationBase
    {
        GenericDevice _garageDoor;

        public GarageDoorRemoteControl(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override async Task Handle()
        {
            switch (_evt.Value)
            {
                case "1":
                    await _garageDoor.DoAction("open");
                    break;
                case "2":
                    await _garageDoor.DoAction("close");
                    break;
            }
        }
        protected override async Task InitDevices()
        {
            _garageDoor = await _hub.GetDeviceByMappedName<GenericDevice>("Demo.GarageDoor");
        }

    }
}
