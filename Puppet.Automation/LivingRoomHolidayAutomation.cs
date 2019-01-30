﻿using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Automation;
using Puppet.Common.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        public override void Handle(CancellationToken token)
        {
            const int minutesBetweenCycles = 5;
            if(_evt.value == "on")
            {
                SwitchRelay LivingRoomXmas1 = 
                    _hub.GetDevice<SwitchRelay>("Switch.LivingRoomXmasScene1") as SwitchRelay;
                SwitchRelay LivingRoomXmas2 = 
                    _hub.GetDevice<SwitchRelay>("Switch.LivingRoomXmasScene2") as SwitchRelay;

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
                SwitchRelay DefaultLivingRoomScene =
                    _hub.GetDevice<SwitchRelay>("Switch.LivingRoomNormalScene") as SwitchRelay;
                DefaultLivingRoomScene.On();
                return;
            }
        }
    }
}