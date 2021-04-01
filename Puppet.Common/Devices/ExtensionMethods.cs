using Puppet.Common.Events;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public static class ExtensionMethods
    {
        public static async Task Speak(this IEnumerable<Speaker> notifiers, string message)
        {
            foreach(var s in notifiers)
            {
                try
                {
                    await s.Speak(message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Speak operation was unsuccessful for speaker {s.Id} -- {ex}");
                }
            }
        }

        public static bool IsAnyOpen(this List<ContactSensor> sensors)
        {
            return !sensors.TrueForAll((s) => s.Status == ContactStatus.Closed);
        }

        public static bool IsAnyOn(this List<SwitchRelay> switches)
        {
            return !switches.TrueForAll((s) => s.Status == SwitchStatus.Off);
        }

        public static bool IsTriggerDevice(this IDevice device, HubEvent evt)
        {
            return (device.Id == evt.DeviceId);
        }

        public static async Task Ensure(this SwitchRelay switchRelay, SwitchStatus desiredState)
        {
            await switchRelay.RefreshState();
            if(switchRelay.Status != desiredState && desiredState != SwitchStatus.Unknown)
            {
                switch (desiredState)
                {
                    case SwitchStatus.Off:
                        await switchRelay.Off();
                        break;

                    case SwitchStatus.On:
                        await switchRelay.On();
                        break;
                }
            }
        }
        
        public static async Task Ensure(this Fan fan, FanSpeed desiredState)
        {
            await fan.RefreshState();
            if (fan.Status != desiredState && desiredState != FanSpeed.Unknown)
            {
                await fan.SetSpeed(desiredState);
            }
        }
    }
}
