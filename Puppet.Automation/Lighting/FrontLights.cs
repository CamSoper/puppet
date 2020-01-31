using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Puppet.Automation.Lighting
{
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    [TriggerDevice("Switch.HolidayDisplay", Capability.Switch)]
    [TriggerDevice("Switch.FrontLightsPower", Capability.Switch)]
    public class FrontLights : AutomationBase
    {
        SwitchRelay _frontLightsPower;
        SwitchRelay _frontLightsWarm;
        SwitchRelay _frontLightsColor;
        SwitchRelay _holidayDisplay;
        
        List<ContactSensor> _doors;
        
        public FrontLights(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task InitDevices()
        {
            _doors = new List<ContactSensor>
            {
                await _hub.GetDeviceByMappedName<ContactSensor>("Contact.FrontDoor"),
                await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1"),
                await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2")
            };

            _frontLightsPower =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsPower");
            _frontLightsColor =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsColorfulScene");
            _frontLightsWarm =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsWarmWhiteScene");
            _holidayDisplay =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.HolidayDisplay");
            
        }

        protected override async Task Handle()
        {
            bool ShouldUseLights = await IsDark(30, -30);

            //Power switch was turned off. Turn it back on and make the lights dark.
            if(_evt.IsOffEvent && IsTriggerDevice(_frontLightsPower))
            {
                await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
                await _frontLightsWarm.Off();
                return;
            }

            //Before we do anything else, make sure Power switch is on
            if(!_frontLightsPower.IsOn)
            {
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
            }

            if((_evt.IsOnEvent && IsTriggerDevice(_holidayDisplay)) 
                  || _evt.IsOnEvent && IsTriggerDevice(_frontLightsPower) && _holidayDisplay.IsOn)
            {
                await DoHolidayStuff();
                return;
            }

            if(_evt.IsOpenEvent && ShouldUseLights)
            {
                await _frontLightsWarm.On();
            }
            else if(_evt.IsClosedEvent && ShouldUseLights)
            {
                if(!_doors.IsAnyOpen())
                {
                    if(_holidayDisplay.IsOn)
                    {
                        await WaitForCancellationAsync(TimeSpan.FromSeconds(10));
                        await DoHolidayStuff();
                    }
                    else
                    {
                        await WaitForCancellationAsync(TimeSpan.FromMinutes(5));
                        await _frontLightsWarm.Off();
                    }
                }
            }
            else
            {
                await _frontLightsWarm.Off();
            }
        }

        private async Task DoHolidayStuff()
        {
            await _frontLightsColor.On();
        }
    }
}
