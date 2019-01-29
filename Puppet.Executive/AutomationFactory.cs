using Puppet.Automation;
using Puppet.Common.Automation;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppet.Executive
{
    public class AutomationFactory
    {
        /// <summary>
        /// Figures out the appropriate implementation of IAutomation based in the data in the event and returns it.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="hub"></param>
        /// <returns>An IEnumberable<IAutomation> containing automations to be run for this event.</returns>
        public static IEnumerable<IAutomation> GetAutomations(HubEvent evt, HomeAutomationPlatform hub)
        {
            foreach(var automation in Assembly.LoadFrom("Puppet.Automation.dll").GetTypes()
                      .Where(t => typeof(IAutomation).IsAssignableFrom(t))
                      .Where(t => t.GetCustomAttributes<TriggerDeviceAttribute>()
                        .Where(a => a.TriggerDeviceId == evt.deviceId
                            && a.Capability.ToString().ToLower() == evt.name)
                        .Count() > 0))
            {
                yield return (IAutomation)Activator.CreateInstance(automation, new Object[] { hub, evt });
            }
        }
    }
}
