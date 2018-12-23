using Puppet.Automation;
using Puppet.Common.Models;
using Puppet.Common.Models.Automation;
using Puppet.Common.Services;

namespace Puppet.Executive
{
    public class AutomationFactory
    {
        /// <summary>
        /// Figures out the appropriate implementation of IAutomation and returns it.
        /// </summary>
        /// <param name="automationName"></param>
        /// <param name="hub"></param>
        /// <returns></returns>
        public static IAutomation GetAutomation(string automationName, HomeAutomationPlatform hub)
        {
            // TODO: Find type dynamically from the Automation DLL at runtime, match by automationName
            switch (automationName)
            {
                case "basementstairwaypowerallowance":
                    return new BasementStairwayPowerAllowance(hub);

                case "garageentrypowerallowance":
                    return new GarageEntryPowerAllowance(hub);

                case "livingroomxmas":
                    return new LivingRoomHolidayAutomation(hub);

                case "safetyalert":
                    return new SafetyAlert(hub);

                case "pantry_light":
                    return new PantryLightAutomation(hub);

                case "octoprint_done":
                    return new OctoprintDoneAutomation(hub);

                case "notifyondoorunlock":
                    return new NotifyOnDoorUnlock(hub);

                default:
                    return null;
            }
        }
    }
}
