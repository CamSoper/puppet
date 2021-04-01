using Microsoft.Azure.ServiceBus;

using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Integration
{
    [RunPerDevice]
    [TriggerDevice("Fan.VirtualAtticFan", Capability.Speed)]
    [TriggerDevice("Switch.AtticFanPower", Capability.Switch)]
    [TriggerDevice("Switch.AtticFanHiLo", Capability.Switch)]
    public class AtticFanController : AutomationBase
    {
        Fan _virtualAtticFan;
        SwitchRelay _atticFanPower;
        SwitchRelay _atticFanHiLo;

        public AtticFanController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected async override Task Handle()
        {
            if(_virtualAtticFan.IsTriggerDevice(_evt))
            {
                //Make reality represent the virtual fan
                switch(_evt.Value.ToLower())
                {
                    case "high":
                        await _atticFanPower.On();
                        await _atticFanHiLo.On();
                        break;

                    case "off":
                        await _atticFanPower.Off();
                        break;

                    default:
                        await _atticFanPower.On();
                        await _atticFanHiLo.Off();
                        break;
                }
            }
            else
            {
                // Make the virtual fan reflect reality
                var switchesAreOn = Tuple.Create<bool, bool>(_atticFanPower.IsOn, _atticFanHiLo.IsOn);
                FanSpeed desiredSpeed = FanSpeed.Unknown;

                switch (switchesAreOn)
                {
                    case (false, false):
                    case (false, true):
                        desiredSpeed = FanSpeed.Off;
                        break;

                    case (true, false):
                        desiredSpeed = FanSpeed.Low;
                        break;

                    case (true, true):
                        desiredSpeed = FanSpeed.High;
                        break;
                }

                if (desiredSpeed != FanSpeed.Unknown &&
                    _virtualAtticFan.Status != desiredSpeed)
                {
                    await _virtualAtticFan.SetSpeed(desiredSpeed);
                }
            }
        } 

        protected async override Task InitDevices()
        {
            _virtualAtticFan = await _hub.GetDeviceByMappedName<Fan>("Fan.VirtualAtticFan");
            _atticFanPower = await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.AtticFanPower");
            _atticFanHiLo = await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.AtticFanHiLo");
        }
    }
}
