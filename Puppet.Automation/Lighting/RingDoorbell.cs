using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Lighting
{
    [TriggerDevice("Switch.RingDoorbell", Capability.Switch)]
    [TriggerDevice("Switch.RingDoorbellMotion", Capability.Switch)]
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    public class RingDoorbell : AutomationBase
    {
        SwitchRelay _frontLights;

        public RingDoorbell(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task Handle()
        {
            if(_evt.IsOpenEvent || _evt.IsClosedEvent)
            {
                return;
            }

            TimeSpan deactivationWait = TimeSpan.FromSeconds(60);
            string timeActivatedKey = this.GetType().ToString();
            TimeSpan timeToWait = TimeSpan.MinValue;
            if (_evt.IsOnEvent && await IsDark(30, -30))
            {
                _hub.StateBag.AddOrUpdate(timeActivatedKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now);
                timeToWait = deactivationWait;
                await _frontLights.On();
            }
            else if (_evt.IsOffEvent)
            {
                DateTime lightActivatedTime =
                    _hub.StateBag.ContainsKey(timeActivatedKey) ? (DateTime)_hub.StateBag[timeActivatedKey] : DateTime.Now;
                timeToWait = deactivationWait - (DateTime.Now - lightActivatedTime);
            }

            Console.WriteLine($"{DateTime.Now} {GetType()} waiting for {timeToWait.TotalSeconds} seconds.");
            await WaitForCancellationAsync(timeToWait);
            await _frontLights.Off();
        }

        protected override async Task InitDevices()
        {
            _frontLights = 
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsWarmWhiteScene");
        }
    }
}
