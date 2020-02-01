using System;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Lighting
{
    [TriggerDevice("Contact.PantryDoor", Capability.Contact)]
    public class Pantry : AutomationBase
    {
        readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        SwitchRelay _pantryLight;
        const string _timeOpenedKey = "PantryOpenedTime";

        public Pantry(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        protected override async Task InitDevices()
        {
            _pantryLight =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.PantryLight");
        }

        /// <summary>
        /// Handles pantry door events coming from the home automation controller.
        /// </summary>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        protected override async Task Handle()
        {
            if (_evt.IsOpenEvent)
            {
                // Turn on the light
                await _pantryLight.On();

                // Remember when we turned on the light for later (when we respond to an off event)
                _hub.StateBag.AddOrUpdate(_timeOpenedKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now); // This is the lambda to just update an existing value with the current DateTime

                // Wait a bit...
                await WaitForCancellationAsync(_interval);
                await _hub.Announce("Please close the pantry door");

                // Wait a bit more...
                await WaitForCancellationAsync(_interval);
                await _hub.Announce("I said, please close the pantry door");

                // Wait a bit longer and then give up...
                await WaitForCancellationAsync(_interval);
                await _hub.Announce("Fine, I'll turn off the light myself.");
                await _pantryLight.Off();
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
                    await _hub.Announce("Thank you for closing the pantry door");
                }
                await _pantryLight.Off();
            }
        }
    }
}