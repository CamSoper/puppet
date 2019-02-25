using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.PantryDoorBackup", Capability.Contact)]
    [TriggerDevice("Switch.PantryAck", Capability.Switch)]
    public class PantryLightAutomation : AutomationBase
    {
        readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        SwitchRelay _pantryLight;
        Speaker _kitchenSpeaker;

        public PantryLightAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _pantryLight =
                _hub.GetDeviceByMappedName<SwitchRelay>("Switch.PantryLight") as SwitchRelay;
            _kitchenSpeaker =
                _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker") as Speaker;
        }

        /// <summary>
        /// Handles pantry door events coming from the home automation controller.
        /// </summary>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        public override async Task Handle(CancellationToken token)
        {
            if (_evt.IsOpenEvent)
            {
                // Turn on the light
                _pantryLight.On();

                // Remember when we turned on the light for later (when we respond to an off event)
                _hub.StateBag.AddOrUpdate("PantryOpened", DateTime.Now,
                    (key, oldvalue) => DateTime.Now); // This is the lambda to just update an existing value with the current DateTime

                // Wait a bit...
                await Task.Delay(_interval, token);
                _kitchenSpeaker.Speak("Please close the pantry door");

                // Wait a bit more...
                await Task.Delay(_interval, token);
                _kitchenSpeaker.Speak("I said, please close the pantry door");

                // Wait a bit longer and then give up...
                await Task.Delay(_interval, token);
                _kitchenSpeaker.Speak("Fine, I'll turn off the light myself.");
                _pantryLight.Off();
            }
            else if (_evt.IsClosedEvent)
            {
                // Has the door been open five minutes?
                DateTime PantryOpenTime =
                    _hub.StateBag.ContainsKey("PantryOpened") ? (DateTime)_hub.StateBag["PantryOpened"] : DateTime.Now;
                if (DateTime.Now - PantryOpenTime > _interval)
                {
                    // It's been open five minutes, so we've nagged by now.
                    // It's only polite to thank them for doing what we've asked!
                    _kitchenSpeaker.Speak("Thank you for closing the pantry door");
                }
                _pantryLight.Off();
            }
            else if (_evt.IsOnEvent &&
                _evt.DeviceId == _hub.LookupDeviceId("Switch.PantryAck"))
            {
                // If you're in the pantry and you don't want it to nag, turn on Switch.PantryAck via Alexa
                // which will cancel any running occurrences of this automation. We'll say something to acknowledge.
                SwitchRelay pantryAck = _hub.GetDeviceById<SwitchRelay>(_evt.DeviceId) as SwitchRelay;
                pantryAck.Off();  // Set the Ack switch back to "off"
                _kitchenSpeaker.Speak("I'm sorry, I didn't know you were busy in there. I'll leave you alone.");
            }
        }
    }
}
