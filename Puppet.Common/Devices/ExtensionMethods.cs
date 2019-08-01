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

        public static bool IsAnyOpen(this IEnumerable<ContactSensor> sensors)
        {
            foreach(var s in sensors)
            {
                if(s.Status == ContactStatus.Open)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
