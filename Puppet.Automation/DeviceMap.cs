using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Automation.DeviceMap
{
    /// <summary>
    /// This namespace contains strings that map my devices to their device numbers in Hubitat.
    /// </summary>
    public sealed class SwitchRelay
    {
        public const string PantryLight = "189";
        public const string Printer3D = "97";
        public const string BasementStairwayLight = "226";
        public const string FrontLightsColorfulScene = "418";
        public const string FrontLightsWarmWhiteScene = "417";
        public const string FrontLightsPower = "338";
        public const string GarageEntry = "257";
        public const string LivingRoomXmasVSwitch = "421";
        public const string LivingRoomXmasScene1 = "384";
        public const string LivingRoomXmasScene2 = "385";
        public const string LivingRoomNormalScene = "386";
    }

    public sealed class Speaker
    {
        public const string KitchenSpeaker = "65";
        public const string WebhookNotifier = "401";
    }

    public sealed class ContactSensor
    {
        public const string FrontDoor = "339";
        public const string PantryDoor = "402";
        public const string PantryDoorBackup = "224";
    }

    public sealed class Lock
    {
        public const string FrontDoorDeadbolt = "388";
    }
}
