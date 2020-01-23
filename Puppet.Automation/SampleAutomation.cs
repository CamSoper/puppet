using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("SampleDevices.SampleDoor", Capability.Contact)]
    public class SampleAutomation : AutomationBase
    {
        readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        SwitchRelay _sampleLight;
        Speaker _sampleSpeaker;
        const string _timeOpenedKey = "PantryOpenedTime";

        public SampleAutomation(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        { }

        protected override async Task InitDevices()
        {
            _sampleLight =
                await _hub.GetDeviceByMappedName<SwitchRelay>("SampleDevices.SampleLight");
            _sampleSpeaker =
                await _hub.GetDeviceByMappedName<Speaker>("SampleDevices.SampleSpeaker");
        }

        /// <summary>
        /// Handles door events coming from the home automation controller.
        /// </summary>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        protected override async Task Handle()
        {
            if (_evt.IsOpenEvent)
            {
                // Turn on the light
                await _sampleLight.On();

                // Remember when we turned on the light for later (when we respond to an off event)
                _hub.StateBag.AddOrUpdate(_timeOpenedKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now); // This is the lambda to just update an existing value with the current DateTime

                // Wait a bit...
                await WaitForCancellationAsync(_interval);
                await _sampleSpeaker.Speak("Please close the door.");

                // Wait a bit more...
                await WaitForCancellationAsync(_interval);
                await _sampleSpeaker.Speak("I said, please close the door.");

                // Wait a bit longer and then give up...
                await WaitForCancellationAsync(_interval);
                await _sampleSpeaker.Speak("Okay, I'll turn off the light myself.");
                await _sampleLight.Off();
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
                    await _sampleSpeaker.Speak("Thank you for closing the pantry door");
                }
                await _sampleLight.Off();
            }
        }
    }
}
