using Puppet.Common.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public enum FanSpeed
    { 
        On,
        Off,
        Auto,
        Low,
        MediumLow,
        Medium,
        MediumHigh,
        High,
        Unknown
    }
    public class Fan : DeviceBase
    {
        public Fan(HomeAutomationPlatform hub, string id) : base(hub, id)
        {
        }

        public FanSpeed Status
        {
            get
            {
                switch (GetState()["speed"])
                {
                    case "on":
                        return FanSpeed.On;

                    case "off":
                        return FanSpeed.Off;

                    case "auto":
                        return FanSpeed.Auto;

                    case "low":
                        return FanSpeed.Low;

                    case "medium-low":
                        return FanSpeed.MediumLow;

                    case "medium":
                        return FanSpeed.Medium;

                    case "medium-high":
                        return FanSpeed.MediumHigh;
                    
                    case "high":
                        return FanSpeed.High;

                    default:
                        return FanSpeed.Unknown;
                }
            }
        }

        public bool IsOn
        {
            get
            {
                return (this.Status != FanSpeed.Off) && (this.Status != FanSpeed.Auto);
            }
        }
        
        public async Task SetSpeed(FanSpeed speed)
        {
            string speedString;

            switch (speed)
            {
                case FanSpeed.MediumLow:
                    speedString = "medium-low";
                    break;

                case FanSpeed.MediumHigh:
                    speedString = "medium-high";
                    break;

                case FanSpeed.Unknown:
                    speedString = "off";
                    break;

                default:
                    speedString = speed.ToString().ToLower();
                    break;
            }

            await _hub.DoAction(this, "setSpeed", new string[] { speedString });
        }
    }
}
