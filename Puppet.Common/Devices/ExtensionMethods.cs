using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Devices
{
    public static class ExtensionMethods
    {
        public static void Speak(this IEnumerable<Speaker> notifiers, string message)
        {
            foreach(var s in notifiers)
            {
                try
                {
                    s.Speak(message);
                }
                catch
                {
                    Console.WriteLine($"Speak operation was unsuccessful for speaker with ID {s.Id}");
                }
            }
        }
    }
}
