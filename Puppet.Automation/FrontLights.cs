using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.FrontDoor", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
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

            if(_evt.IsOpenEvent && ShouldUseLights)
            {
                if(_holidayDisplay.Status == SwitchStatus.On)
                {
                    await _frontLightsWarm.On();
                }
                else
                {
                    await _frontLightsPower.On();
                }
                
            }
            else if(_evt.IsClosedEvent && ShouldUseLights)
            {
                if(!_doors.IsAnyOpen() && _frontLightsPower.Status == SwitchStatus.On)
                {
                    if(_holidayDisplay.Status == SwitchStatus.On)
                    {
                        await WaitForCancellationAsync(TimeSpan.FromSeconds(10));
                        await _frontLightsColor.On();
                    }
                    else
                    {
                        await WaitForCancellationAsync(TimeSpan.FromMinutes(10));
                        await _frontLightsPower.Off();
                    }
                }
            }
            else
            {
                await _frontLightsPower.Off();
            }
        }
    }
}
