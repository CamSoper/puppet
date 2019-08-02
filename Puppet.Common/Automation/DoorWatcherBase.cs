using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    public abstract class DoorWatcherBase : AutomationBase
    {
        public TimeSpan HowLong { get; set; }
        public List<Speaker> NotificationDevices { get; set; }
        public string NotificationFormat { get; set; }
        public int NumberOfNotifications { get; set; }
        public bool NotifyOnClose { get; set; }

        public DoorWatcherBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {}

        protected override async Task Handle()
        {
            if (NotificationDevices.Count == 0)
            {
                return;
            }

            if (_evt.IsOpenEvent)
            {
                if (NumberOfNotifications < 1) NumberOfNotifications = 1;
                if (String.IsNullOrEmpty(NotificationFormat)) NotificationFormat = @"{0} has been open {1} minutes.";

                for (int i = 0; i < NumberOfNotifications; i++)
                {
                    await WaitForCancellationAsync(HowLong);
                    await NotificationDevices.Speak(String.Format(NotificationFormat,
                        _evt.DisplayName, HowLong.TotalMinutes * (i + 1), HowLong.TotalSeconds * (i + 1)));
                    // There's an unused string parameter being passed into String.Format.
                    // That way the deriving class can set the NotificationFormat to mention
                    // either "seconds" or "minutes."
                }
            }
            else if (_evt.IsClosedEvent && NotifyOnClose)
            {
                await NotificationDevices.Speak($"{_evt.DisplayName} is closed.");
            }
        }
    }
}
