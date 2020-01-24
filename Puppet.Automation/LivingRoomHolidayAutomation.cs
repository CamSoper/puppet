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
        /// Cycles living room lights between two scenes while a v-switch is turned on
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="token"></param>
        protected override async Task Handle()
        {
            TimeSpan timeBetweenCycles = TimeSpan.FromMinutes(5);
            if (_evt.IsOnEvent)
            {
                SwitchRelay LivingRoomXmas1 =
                    await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.LivingRoomXmasScene1");
                SwitchRelay LivingRoomXmas2 =
                    await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.LivingRoomXmasScene2");

                while (true)
                {
                    await LivingRoomXmas1.On();
                    await WaitForCancellationAsync(timeBetweenCycles);
                    await LivingRoomXmas2.On();
                    await WaitForCancellationAsync(timeBetweenCycles);
                }
            }
            else
            {
                SwitchRelay DefaultLivingRoomScene =
                    await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.LivingRoomNormalScene");
                await DefaultLivingRoomScene.On();
                return;
            }
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }
    }
}
