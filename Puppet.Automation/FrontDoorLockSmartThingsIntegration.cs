using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Puppet.Automation
{
    [RunPerDevice]
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    [TriggerDevice("Lock.SmartThingsFrontDoorDeadbolt", Capability.Lock)]
    public class FrontDoorLockSmartThingsIntegration : AutomationBase
    {

        LockDevice _frontDoorLock;
        LockDevice _smartthingsLock;
        Uri _endpoint;

        public FrontDoorLockSmartThingsIntegration(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _endpoint = new Uri(_hub.Configuration["FrontDoorLockSmartThingsIntegrationEndpoint"]);
        }

        protected override async Task InitDevices()
        {

            _frontDoorLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.FrontDoorDeadbolt");

            _smartthingsLock =
                await _hub.GetDeviceByMappedName<LockDevice>("Lock.SmartThingsFrontDoorDeadbolt");

        }

        protected override async Task Handle()
        {
            if (_evt.DeviceId == _frontDoorLock.Id)
            {
                string bodyText;
                if (_evt.Value == "unlocking")
                {
                    bodyText = JsonConvert.SerializeObject(new { lockAction = "unlock" });

                }
                else if (_evt.Value == "locking")
                {
                    bodyText = JsonConvert.SerializeObject(new { lockAction = "lock" });
                }
                else
                {
                    return;
                }

                HttpContent contentPost = new StringContent(bodyText, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(_endpoint, contentPost);
                    response.EnsureSuccessStatusCode();
                }
                
            }
            else if (_evt.DeviceId == _hub.LookupDeviceId("Lock.SmartThingsFrontDoorDeadbolt"))
            {
                if (_evt.Value == "locked")
                {
                    await _frontDoorLock.DoAction("confirmLocked");
                }
                else if (_evt.Value == "unlocked")
                {
                    await _frontDoorLock.DoAction("confirmUnlocked");
                }
                return;
            }
        }
    }
}
