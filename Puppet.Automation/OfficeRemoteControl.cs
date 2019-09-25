using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Demo.Remote", Capability.Held)]
    [TriggerDevice("Demo.Remote", Capability.Pushed)]
    public class OfficeRemoteControl : AutomationBase
    {
        Speaker _speaker;
        public OfficeRemoteControl(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override async Task InitDevices()
        {
            _speaker = await _hub.GetDeviceByMappedName<Speaker>("Demo.Speaker");
        }

        protected override async Task Handle()
        {
            switch(_evt.Value, _evt.IsButtonPushedEvent)
            {
                case ("1", true):
                    await _speaker.Speak("Your father requires quiet, please.");
                    break;
                case ("2", true):
                    await _speaker.Speak("Your father says he hears you, but is unable to answer.");
                    break;
                case ("3", true):
                    await _speaker.Speak("Please let the dogs in.");
                    break;
                case ("4", true):
                    await _speaker.Speak("Whatever you are doing, it is frustrating your father. Please correct it now or there will be consequences.");
                    break;
                case ("4", false):
                    await _speaker.Speak("I said, there will be consequences. BIG. CONSEQUENCES.");
                    break;
            }
        }
    }
}
