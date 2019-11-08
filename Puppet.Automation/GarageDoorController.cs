using Microsoft.Azure.ServiceBus;
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
    [RunPerDevice]
    [TriggerDevice("Demo.GarageDoorSensor", Capability.Contact)]
    [TriggerDevice("Demo.GarageDoor", Capability.Door)]
    public class GarageDoorController : AutomationBase
    {
        GenericDevice _garageDoor;

        public GarageDoorController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }
        
        protected override async Task InitDevices()
        {
            _garageDoor = await _hub.GetDeviceByMappedName<GenericDevice>("Demo.GarageDoor");
        }

        protected override async Task Handle()
        {
            if (_evt.DeviceId != _garageDoor.Id)
            {
                if (_evt.IsOpenEvent)
                {
                    await _garageDoor.DoAction("confirmOpen");
                }
                else
                {
                    await _garageDoor.DoAction("confirmClosed");
                }
            }
            else
            {
                if(_evt.Value == "opening" || _evt.Value == "closing")
                {
                    Console.WriteLine($"{DateTime.Now} Sending signal to cycle garage door.");
                    await SendCycleCommand();
                }
            }
        }

        private async Task SendCycleCommand()
        {
            string svcBusConn = _hub.Configuration["GarageDoorConnectionString"];
            ServiceBusConnectionStringBuilder builder = 
                new ServiceBusConnectionStringBuilder(svcBusConn);
            QueueClient client = new QueueClient(builder);
            await client.SendAsync(
                new Message()
                {
                    Label = "open sesame"
                });
        }


    }
}
