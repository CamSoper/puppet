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
    }
}
