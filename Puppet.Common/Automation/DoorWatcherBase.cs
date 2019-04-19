using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    public class DoorWatcherBase : AutomationBase
    {
        public TimeSpan HowLong { get; set; }
        public List<Speaker> NotificationDevices { get; set; }
        public string NotificationFormat { get; set; }
        public int NumberOfNotifications { get; set; }
        public bool NotifyOnClose { get; set; }

        public DoorWatcherBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {

        }

        protected override async Task Handle()
        {
            if (_evt.IsOpenEvent)
            {
                if (NumberOfNotifications < 1) NumberOfNotifications = 1;
                if (String.IsNullOrEmpty(NotificationFormat)) NotificationFormat = @"{0} has been open {1} minutes.";

                for (int i = 0; i < NumberOfNotifications; i++)
                {
                    await WaitForCancellationAsync(HowLong);
                    NotificationDevices.Speak(String.Format(NotificationFormat,
                        _evt.DisplayName, HowLong.TotalMinutes * (i + 1), HowLong.TotalSeconds * (i + 1)));
                    // Yes, there's an extra parameter being passed into String.Format.
                    // I wanted to ensure the NotificationFormat has some flexibility
                    // insofar as the text that is passed in.
                }
            }
            else if (_evt.IsClosedEvent && NotifyOnClose)
            {
                NotificationDevices.Speak($"{_evt.DisplayName} is closed.");
            }
        }
    }
}
