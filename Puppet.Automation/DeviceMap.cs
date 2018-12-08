using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Automation.DeviceMap
{
    /// <summary>
    /// This namespace contains enums that map my devices to their device numbers in Hubitat.
    /// </summary>
    public sealed class SwitchRelay
    {
        public const string PantryLight = "189";
        public const string ThreeDPrinter = "97";
        public const string BasementStairwayLight = "226";
    }

    public sealed class Speaker
    {
        public const string KitchenSpeaker = "65";
        public const string BedroomEcho = "197";
        public const string DenEcho = "200";
        public const string CadeEcho = "202";
        public const string CollinEcho = "201";
        public const string KitchenEcho = "199";
        public const string LivingRoomEcho = "203";
        public const string OfficeEcho = "198";
    }
}
