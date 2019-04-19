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

        public NotifyOnMailArrival(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
        }

        protected override Task Handle()
        {
            bool ShouldNotify = 
                _hub.StateBag.ContainsKey(_mailboxNotifyKey) ? 
                    ((DateTime)_hub.StateBag[_mailboxNotifyKey] < DateTime.Now.AddMinutes(-5)) : 
                    true;

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

            return Task.CompletedTask;
        }
    }
}
