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

                // Wait a bit...
                await WaitForCancellationAsync(TimeSpan.FromMinutes(10));
                await _hub.Announce("I'm turning off the pantry lights in one minute.");

                // Wait a bit longer and then give up...
                await WaitForCancellationAsync(TimeSpan.FromMinutes(1));
                await _pantryLight.Off();
            }
            else
            {
                await _pantryLight.Off();
            }
        }
    }
}