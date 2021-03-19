using Puppet.Automation.Services.Notifiers;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Notifiers;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation.Miscellaneous
{
    [TriggerDevice("Button.OfficeRemote", Capability.Held)]
    [TriggerDevice("Button.OfficeRemote", Capability.Pushed)]
    public class OfficeRemoteControl : AutomationBase
    {
        INotifier _alexaNotifier;

        public OfficeRemoteControl(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _alexaNotifier = new HassAlexaNotifier(_hub.Configuration, new string[] { "Everywhere" });
        }

        protected override async Task Handle()
        {
            if (_evt.IsButtonPushedEvent)
            {
                switch (_evt.Value)
                {
                    case "1":
                        await _alexaNotifier.SendNotification("Your father requires quiet, please.");
                        break;
                    case "2":
                        await _alexaNotifier.SendNotification("Your father says he hears you, but is unable to answer.");
                        break;
                    case "3":
                        await _alexaNotifier.SendNotification("Please let the dogs in.");
                        break;
                    case "4":
                        await _alexaNotifier.SendNotification("Whatever you are doing, it is frustrating your father. Please correct it now or there will be consequences.");
                        break;
                }
            }
            else
            {
                switch (_evt.Value)
                {
                    case "1":
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        await _alexaNotifier.SendNotification("I said, there will be consequences. BIG. CONSEQUENCES.");
                        break;
                }
            }
        }
        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }

    }
}
