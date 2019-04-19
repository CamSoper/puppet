using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation
{
    [TriggerDevice("Lock.FrontDoorDeadbolt", Capability.Lock)]
    public class NotifyOnDoorUnlock : AutomationBase
    {
        public NotifyOnDoorUnlock(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override Task Handle()
        {
            if (_evt.IsUnLockedEvent)
            {
                if (_evt.DescriptionText.Contains("was unlocked by"))
                {
                    Speaker notifier = _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier") as Speaker;
                    notifier.Speak($"{_evt.DescriptionText}.");
                }
            }

            return Task.CompletedTask;
        }
    }
}