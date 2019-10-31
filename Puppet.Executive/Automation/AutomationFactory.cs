using Microsoft.Extensions.Configuration;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppet.Executive.Automation
{
    public class AutomationFactory
    {
        private const string _automationAssembly = "Puppet.Automation.dll";
        private IConfiguration _config;

        public AutomationFactory(IConfiguration configuration ) {
            _config = configuration;
        }

        /// <summary>
        /// Figures out the appropriate implementation of IAutomation based on the data in the event and returns it.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="hub"></param>
        /// <returns>An IEnumerable&lt;IAutomation&gt; containing the automations to be run for this event.</returns>
        public IEnumerable<IAutomation> GetAutomations(HubEvent evt, HomeAutomationPlatform hub)
        {
            /*
             *  Get the types from the assembly
             *      where the type implements IAutomation and
             *          the type has trigger attributes
             *              where the trigger attribute names a mapped device that matches the device that caused the event
             *                  and the attribute also names a Capability that matches the device that caused the event
             *          and the count of the matching trigger attributes is greater than 0
             */
            IEnumerable<Type> typeCollection = Assembly.LoadFrom(_automationAssembly).GetTypes() 
                .Where(t => typeof(IAutomation).IsAssignableFrom(t) && 
                    (t.GetCustomAttributes<TriggerDeviceAttribute>() 
                        .Where(a => hub.LookupDeviceId(a.DeviceMappedName) == evt.DeviceId &&
                            a.Capability.ToString().ToLower() == evt.Name))
                    .Count() > 0);
            foreach (Type automation in typeCollection)
            {
                var thing = Activator.CreateInstance(automation, new Object[] { hub, evt });
                if (thing is IAutomation automationSource)
                    yield return automationSource;
            }
        }
    }
}
