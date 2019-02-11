using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Switch.LivingRoomXmasVSwitch", Capability.Switch)]
    public class LivingRoomHolidayAutomation : AutomationBase
    {
        public LivingRoomHolidayAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        /// <summary>
        /// Cycles living room lights between two scenes while a vswitch is turned on
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="token"></param>
        public override async Task Handle(CancellationToken token)
        {
            TimeSpan timeBetweenCycles = TimeSpan.FromMinutes(5);
            if (_evt.IsOnEvent)
            {
                SwitchRelay LivingRoomXmas1 =
                    _hub.GetDeviceByName<SwitchRelay>("Switch.LivingRoomXmasScene1") as SwitchRelay;
                SwitchRelay LivingRoomXmas2 =
                    _hub.GetDeviceByName<SwitchRelay>("Switch.LivingRoomXmasScene2") as SwitchRelay;

                while (true)
                {
                    LivingRoomXmas1.On();
                    await Task.Delay(timeBetweenCycles, token);
                    LivingRoomXmas2.On();
                    await Task.Delay(timeBetweenCycles, token);
                }
            }
            else
            {
                SwitchRelay DefaultLivingRoomScene =
                    _hub.GetDeviceByName<SwitchRelay>("Switch.LivingRoomNormalScene") as SwitchRelay;
                DefaultLivingRoomScene.On();
                return;
            }
        }
    }
}
