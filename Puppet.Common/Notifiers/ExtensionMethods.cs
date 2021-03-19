using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Notifiers
{
    public static class ExtensionMethods
    {
        public static async Task SendMessage(this IEnumerable<INotifier> notifiers, string message)
        {
            foreach (var n in notifiers)
            {
                try
                {
                    await n.SendNotification(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Notify operation was unsuccessful for speaker {n} -- {ex}");
                }
            }
        }
    }
}
