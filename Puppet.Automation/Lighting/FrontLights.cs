using Puppet.Automation.Services.HomeAssistant;
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
    [TriggerDevice("Switch.FrontLightsGroup", Capability.Switch)]
    public class FrontLights : AutomationBase
    {
        SwitchRelay _frontLightsPower;
        SwitchRelay _frontLightsGroup;
        SwitchRelay _holidayDisplay;
        
        List<ContactSensor> _doors;

        HassSceneService _hassSceneService;

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
            _frontLightsGroup =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsGroup");
            _holidayDisplay =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.HolidayDisplay");

            _hassSceneService = new HassSceneService(_hub.Configuration);
            
        }

        protected override async Task Handle()
        {
            bool ShouldUseLights = true; //await IsDark(30, -30);

            //Power switch was turned off. Turn it back on and make the lights dark.
            if (_evt.IsOffEvent && _frontLightsPower.IsTriggerDevice(_evt))
            {
                await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
                await _frontLightsGroup.Off();
            }

            //Before we do anything else, make sure Power switch is on
            if (!_frontLightsPower.IsOn)
            {
                await _frontLightsPower.On();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(3));
            }


            if ((_evt.IsOnEvent && _holidayDisplay.IsTriggerDevice(_evt))
                    || (_evt.IsOnEvent && _frontLightsPower.IsTriggerDevice(_evt) && _holidayDisplay.IsOn)
                    || (_evt.IsOffEvent && _frontLightsGroup.IsTriggerDevice(_evt) && _holidayDisplay.IsOn))
            {
                await DoHolidayStuff();
                return;
            }

            if(_evt.IsOpenEvent && ShouldUseLights)
            {
                await _hassSceneService.ApplyScene(Scene.Warm_Front_Lights);
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
                        await _frontLightsGroup.Off();
                    }
                }
            }
            else
            {
                await _frontLightsGroup.Off();
            }
        }

        private async Task DoHolidayStuff()
        {
            await _frontLightsGroup.On();
            await _hassSceneService.ApplyScene(Scene.Colorful_Front_Lights);
        }
    }
}
