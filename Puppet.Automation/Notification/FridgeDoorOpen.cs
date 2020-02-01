﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Automation.Notification
{
    [RunPerDevice]
    [TriggerDevice("Contact.GarageFreezer", Capability.Contact)]
    [TriggerDevice("Contact.GarageFridge", Capability.Contact)]
    [TriggerDevice("Contact.KitchenFridge", Capability.Contact)]
    [TriggerDevice("Contact.BasementFreezer", Capability.Contact)]
    public class FridgeDoorOpen : DoorWatcherBase
    {
        public FridgeDoorOpen(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            HowLong = TimeSpan.FromMinutes(1);
            NumberOfNotifications = 2;
            MakeAnnouncement = true;
            PushNotification = true;
        }

        protected override Task InitDevices()
        {
            return Task.CompletedTask;
        }

    }
}
