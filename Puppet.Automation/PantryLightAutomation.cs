using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using Puppet.Common.Automation;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Puppet.Automation
{
    [TriggerDevice(DeviceMap.ContactSensor.PantryDoor, Capability.Contact)]
    public class PantryLightAutomation : AutomationBase
    {
        SwitchRelay _pantryLight;
        Speaker _kitchenSpeaker;
        HomeAutomationPlatform _hub;
        HubEvent _evt;

        public PantryLightAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _pantryLight = new SwitchRelay(hub, DeviceMap.SwitchRelay.PantryLight);
            _kitchenSpeaker = new Speaker(hub, DeviceMap.Speaker.KitchenSpeaker);
            _hub = hub;
            _evt = evt;
        }

        /// <summary>
        /// Handles pantry door events coming from the home automation controller.
        /// </summary>
        /// <param name="evt">The event passed from the automation controller.
        /// In this case, the pantry door opening/closing.</param>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled. </param>
        public override void Handle(CancellationToken token)
        {
            if(_evt.value == "open")
            {
                // Turn on the light
                _pantryLight.On();

                // Remember when we turned on the light in case we need to know later
                _hub.StateBag.AddOrUpdate("PantryOpened", DateTime.Now, 
                    (key, oldvalue) => DateTime.Now); // This is the lambda to just update an existing value with the current DateTime

                // Wait a bit
                Task.Delay(TimeSpan.FromMinutes(5)).Wait();
                if (token.IsCancellationRequested) return;
                _kitchenSpeaker.Speak("Please close the pantry door");

                // Wait a bit more
                Task.Delay(TimeSpan.FromMinutes(5)).Wait();
                if (token.IsCancellationRequested) return;
                _kitchenSpeaker.Speak("I said, please close the pantry door");

                // Wait a bit longer...
                Task.Delay(TimeSpan.FromMinutes(5)).Wait();
                if (token.IsCancellationRequested) return;
                _kitchenSpeaker.Speak("Fine, I'll do it myself.");
                _pantryLight.Off();
            }
            else
            {
                // Has the door been open five minutes?
                DateTime PantryOpenTime = 
                    _hub.StateBag.ContainsKey("PantryOpened") ? (DateTime)_hub.StateBag["PantryOpened"] : DateTime.Now;
                if(DateTime.Now - PantryOpenTime > TimeSpan.FromMinutes(5))
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
