using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.PantryDoor", Capability.Contact)]
    public class PantryLightAutomation : AutomationBase
    {
        readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        SwitchRelay _pantryLight;
        Speaker _kitchenSpeaker;
        const string _timeOpenedKey = "PantryOpenedTime";

        public PantryLightAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        private async Task InitDevices()
        {
            _pantryLight =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.PantryLight");
            _kitchenSpeaker =
                await _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker");
        }

        /// <summary>
        /// Handles pantry door events coming from the home automation controller.
        /// </summary>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        protected override async Task Handle()
        {
            await InitDevices();

            if (_evt.IsOpenEvent)
            {
                // Turn on the light
                _pantryLight.On();

                // Remember when we turned on the light for later (when we respond to an off event)
                _hub.StateBag.AddOrUpdate(_timeOpenedKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now); // This is the lambda to just update an existing value with the current DateTime

                // Wait a bit...
                await WaitForCancellationAsync(_interval);
                _kitchenSpeaker.Speak("Please close the pantry door");

                // Wait a bit more...
                await WaitForCancellationAsync(_interval);
                _kitchenSpeaker.Speak("I said, please close the pantry door");

                // Wait a bit longer and then give up...
                await WaitForCancellationAsync(_interval);
                _kitchenSpeaker.Speak("Fine, I'll turn off the light myself.");
                _pantryLight.Off();
            }
            else
            {
                // Has the door been open five minutes?
                DateTime PantryOpenTime =
                    _hub.StateBag.ContainsKey(_timeOpenedKey) ? (DateTime)_hub.StateBag[_timeOpenedKey] : DateTime.Now;
                if (DateTime.Now - PantryOpenTime > _interval)
                {
                    // It's been open five minutes, so we've nagged by now.
                    // It's only polite to thank them for doing what we've asked!
                    _kitchenSpeaker.Speak("Thank you for closing the pantry door");
                }
                _pantryLight.Off();
            }
        }
    }
}
