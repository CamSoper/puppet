using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation
{
    [TriggerDevice("Contact.Mailbox", Capability.Contact)]
    public class NotifyOnMailArrival : AutomationBase
    {
        const string _mailboxNotifyKey = "MailboxNotificationTime";
        const string _mailboxLastEventTimeKey = "MailboxLastEventTime";
        const int _debounceTimeoutMs = 1000;

        public NotifyOnMailArrival(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected async override Task Handle()
        {
            // I don't need mailbox notifications in the middle of the night.
            if(DateTime.Now.Hour < 7 || DateTime.Now.Hour > 22)
            {
                return;
            }

            // Mailbox sensor is flaky and sends weird events sometimes.
            // They're usually stacked sub-second, so this should fix that.
            await WaitForCancellationAsync(TimeSpan.FromMilliseconds(_debounceTimeoutMs));
            if(_hub.StateBag.ContainsKey(_mailboxLastEventTimeKey))
            {
                if(((DateTime)_hub.StateBag[_mailboxLastEventTimeKey]).AddMilliseconds(_debounceTimeoutMs) > DateTime.Now)
                {
                    Console.WriteLine($"{DateTime.Now} {GetType()} detected mailbox bounce. Ignoring.");
                    return;
                }
            }
            _hub.StateBag.AddOrUpdate(_mailboxLastEventTimeKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now);

            // Max of one notification every 5 minutes.
            bool ShouldNotify = 
                _hub.StateBag.ContainsKey(_mailboxNotifyKey) ? 
                    (((DateTime)_hub.StateBag[_mailboxNotifyKey]).AddMinutes(5) < DateTime.Now) : 
                    true;

            // Send the notification.
            if (ShouldNotify)
            {
                _hub.StateBag.AddOrUpdate(_mailboxNotifyKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now);

                List<Speaker> notificationDevices =
                    new List<Speaker>() {
                        _hub.GetDeviceByMappedName<Speaker>("Speaker.WebhookNotifier") as Speaker,
                        _hub.GetDeviceByMappedName<Speaker>("Speaker.KitchenSpeaker") as Speaker
                    };
                notificationDevices.Speak("There is activity at the mailbox.");

            }
        }
    }
}
