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
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    [TriggerDevice("Switch.HolidayDisplay", Capability.Switch)]
    [TriggerDevice("Switch.FrontLightsPower", Capability.Switch)]
    [TriggerDevice("Switch.FrontLightsWarmWhiteScene", Capability.Switch)]
    public class FrontLights : AutomationBase
    {
        SwitchRelay _frontLightsPower;
        SwitchRelay _frontLightsWarm;
        SwitchRelay _frontLightsColor;
        SwitchRelay _frontLightsDark;
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
            _frontLightsDark =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsOffScene");
            _holidayDisplay =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.HolidayDisplay");
            
        }

        protected override async Task Handle()
        {
            bool ShouldUseLights = await IsDark(30, -30);

            //Power switch was turned off. Turn it back on and make the lights dark.
            if(_evt.IsOffEvent && _evt.DeviceId == _frontLightsPower.Id)
            {
                await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
                await _frontLightsDark.On();
                return;
            }

            //Before we do anything else, make sure Power switch is on
            if(_frontLightsPower.Status == SwitchStatus.Off)
            {
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
            }

            if(_evt.IsOnEvent && IsTriggerDevice(_holidayDisplay))
            {
                await _frontLightsColor.On();
                return;
            }

            if(_evt.IsOnEvent && IsTriggerDevice(_frontLightsWarm))
            {
                await _frontLightsWarm.On();
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
                    if(_holidayDisplay.Status == SwitchStatus.On)
                    {
                        await WaitForCancellationAsync(TimeSpan.FromSeconds(10));
                        await _frontLightsColor.On();
                    }
                    else
                    {
                        await WaitForCancellationAsync(TimeSpan.FromMinutes(10));
                        await _frontLightsDark.On();
                    }
                }
            }
            else
            {
                await _frontLightsDark.On();
            }
        }
    }
}
