using Microsoft.Azure.ServiceBus;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Integration
{
    [RunPerDevice]
    [TriggerDevice("Contact.GarageDoor1", Capability.Contact)]
    [TriggerDevice("Contact.GarageDoor2", Capability.Contact)]
    [TriggerDevice("Garage.Door1Opener", Capability.Door)]
    [TriggerDevice("Garage.Door2Opener", Capability.Door)]
    public class GarageDoorController : AutomationBase
    {
        List<GenericDevice> _garageDoors = new List<GenericDevice>();
        List<ContactSensor> _sensors = new List<ContactSensor>();
      
        public GarageDoorController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override async Task InitDevices()
        {
            _garageDoors.Add(await _hub.GetDeviceByMappedName<GenericDevice>("Garage.Door1Opener"));
            _sensors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor1"));

            _garageDoors.Add(await _hub.GetDeviceByMappedName<GenericDevice>("Garage.Door2Opener"));
            _sensors.Add(await _hub.GetDeviceByMappedName<ContactSensor>("Contact.GarageDoor2"));
        }

        protected override async Task Handle()
        {
            if (_sensors.Where(s => s.Id == _evt.DeviceId).Count() > 0)
            {
                int doorIndex = _sensors.IndexOf(_sensors.Where(s => s.Id == _evt.DeviceId).FirstOrDefault());
                if (_evt.IsOpenEvent)
                {
                    await _garageDoors[doorIndex].DoAction("confirmOpen");
                }
                else
                {
                    await _garageDoors[doorIndex].DoAction("confirmClosed");
                }
            }
            else
            {
                int doorIndex = _garageDoors.IndexOf(_garageDoors.Where(s => s.Id == _evt.DeviceId).FirstOrDefault());

                if (_evt.Value == "opening" || _evt.Value == "closing")
                {
                    Console.WriteLine($"{DateTime.Now} Sending signal to cycle garage door {doorIndex + 1}.");
                    await SendCycleCommand(doorIndex + 1);

                    // Wait for this task to be cancelled when the cycling completes and a new event is triggered
                    await WaitForCancellationAsync(TimeSpan.FromSeconds(30));
                    await _hub.Push($"Garage door {doorIndex + 1} is in an unknown state! Please check it.");
                    return;
                }
            }
        }

        private async Task SendCycleCommand(int garageDoor)
        {
            string svcBusConn = _hub.Configuration["GarageDoorConnectionString"];
            ServiceBusConnectionStringBuilder builder =
                new ServiceBusConnectionStringBuilder(svcBusConn);
            QueueClient client = new QueueClient(builder);
            var message = new Message();
            message.Label = "open sesame";
            message.UserProperties.Add("door", garageDoor);
            await client.SendAsync(message);
        }


    }
}