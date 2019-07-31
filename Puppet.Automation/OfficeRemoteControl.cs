﻿using Puppet.Common.Automation;
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
    [TriggerDevice("Button.OfficeRemote", Capability.Held)]
    [TriggerDevice("Button.OfficeRemote", Capability.Pushed)]
    public class OfficeRemoteControl : AutomationBase
    {
        public OfficeRemoteControl(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override Task Handle()
        {
            Speaker speaker = _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker");
            if(_evt.IsButtonPushedEvent)
            {
                switch(_evt.Value)
                {
                    case "1":
                        speaker.Speak("Your father requires quiet, please.");
                        break;
                    case "2":
                        speaker.Speak("Your father says he hears you, but is unable to answer.");
                        break;
                    case "3":
                        speaker.Speak("Please let the dogs in.");
                        break;
                    case "4":
                        speaker.Speak("Whatever you are doing, it is frustrating your father. Please correct it now or there will be consequences.");
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
                        speaker.Speak("I said, there will be consequences. BIG. CONSEQUENCES.");
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
