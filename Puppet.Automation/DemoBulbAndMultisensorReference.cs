using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Demo.MultiSensor", Capability.Contact)]
    class DemoBulbAndMultisensorReference : AutomationBase
    {
        SwitchRelay _bulb;
        Speaker _speaker;
        TimeSpan _interval;

        public DemoBulbAndMultisensorReference(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        protected override async Task Handle()
        {
            if(_evt.IsOpenEvent)
            {
                await _bulb.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                await _speaker.Speak("Please close the door.");
            }
            else
            {
                await _bulb.Off();
            }
        }

        protected override async Task InitDevices()
        {
            _bulb = await _hub.GetDeviceByMappedName<SwitchRelay>("Demo.Bulb");
            _speaker = await _hub.GetDeviceByMappedName<Speaker>("Demo.Speaker");
        }
    }
}
