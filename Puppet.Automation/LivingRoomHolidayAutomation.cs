using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Models.Automation;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    public class LivingRoomHolidayAutomation : IAutomation
    {
        private readonly HomeAutomationPlatform _hub;

        public LivingRoomHolidayAutomation(HomeAutomationPlatform hub)
        {
            _hub = hub;
        }

        /// <summary>
        /// Cycles living room lights between two scenes while a vswitch is turned on
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="token"></param>
        public void Handle(HubEvent evt, CancellationToken token)
        {
            const int minutesBetweenCycles = 5;
            if(evt.value == "on")
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
