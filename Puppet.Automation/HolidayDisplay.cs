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
    [TriggerDevice("Switch.HolidayDisplay", Capability.Switch)]
    public class HolidayDisplay : AutomationBase
    {
        SwitchRelay _frontLightsPower;
        SwitchRelay _frontLightsWarm;
        SwitchRelay _frontLightsColor;
        SwitchRelay _holidayDisplay;
        SwitchRelay _outdoorPlug;
                
        public HolidayDisplay(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task InitDevices()
        {

            _frontLightsPower =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsPower");
            _frontLightsColor =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsColorfulScene");
            _frontLightsWarm =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.FrontLightsWarmWhiteScene");
            _holidayDisplay =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.HolidayDisplay");
            _outdoorPlug =
                await _hub.GetDeviceByMappedName<SwitchRelay>("Switch.OutdoorPlug");
            
        }

        protected override async Task Handle()
        {
            if(_evt.IsOnEvent)
            {
                if(_frontLightsPower.Status == SwitchStatus.Off)
                {
                    await _frontLightsPower.On();
                    await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                }
                await _frontLightsColor.On();
                await _outdoorPlug.On();
            }
            else
            {
                if(_frontLightsPower.Status == SwitchStatus.Off)
                {
                    await _frontLightsPower.On();
                    await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                }
                await _frontLightsWarm.On();
                await _outdoorPlug.Off();
                await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
                await _frontLightsPower.Off();
            }

        }
    }
}
