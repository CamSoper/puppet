using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice(DeviceMap.SwitchRelay.LivingRoomXmasVSwitch)]
    public class LivingRoomHolidayAutomation : AutomationBase
    {
        private readonly HomeAutomationPlatform _hub;
        private readonly HubEvent _evt;

        public LivingRoomHolidayAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _hub = hub;
            _evt = evt;
        }

        /// <summary>
        /// Cycles living room lights between two scenes while a vswitch is turned on
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="token"></param>
        public override void Handle(CancellationToken token)
        {
            const int minutesBetweenCycles = 5;
            if(_evt.value == "on")
            {
                var LivingRoomXmas1 = new SwitchRelay(_hub, DeviceMap.SwitchRelay.LivingRoomXmasScene1);
                var LivingRoomXmas2 = new SwitchRelay(_hub, DeviceMap.SwitchRelay.LivingRoomXmasScene2);

                while(true)
                {
                    LivingRoomXmas1.On();
                    Task.Delay(TimeSpan.FromMinutes(minutesBetweenCycles)).Wait();
                    if (token.IsCancellationRequested) return;
                    LivingRoomXmas2.On();
                    Task.Delay(TimeSpan.FromMinutes(minutesBetweenCycles)).Wait();
                    if (token.IsCancellationRequested) return;
                }
            }
            else
            {
                var DefaultLivingRoomScene =
                    new SwitchRelay(_hub, DeviceMap.SwitchRelay.LivingRoomNormalScene);
                DefaultLivingRoomScene.On();
                return;
            }
        }
    }
}
