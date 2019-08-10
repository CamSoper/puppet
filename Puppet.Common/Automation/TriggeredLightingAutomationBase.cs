using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Common.Automation
{
    /// <summary>
    /// Base class for turning on lights in response to a door opening or other switch being turned on.
    /// </summary>
    public abstract class TriggeredLightingAutomationBase : AutomationBase
    {
        public TimeSpan DeactivationWait { get; set; }
        public bool EnableDeactivation { get; set; }
        public List<SwitchRelay> SwitchesToActivate { get; set; }
        public TriggeredLightingAutomationBase(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            SwitchesToActivate = new List<SwitchRelay>();
        }

        protected async override Task Handle()
        {
            if (SwitchesToActivate.Count == 0)
            {
                return;
            }

            string timeActivatedKey = this.GetType().ToString();
            TimeSpan timeToWait = TimeSpan.MinValue;
            if (_evt.IsOpenEvent || _evt.IsOnEvent)
            {
                foreach (var s in SwitchesToActivate)
                {
                    await s.On();
                }
                _hub.StateBag.AddOrUpdate(timeActivatedKey, DateTime.Now,
                    (key, oldvalue) => DateTime.Now);
                timeToWait = DeactivationWait;
            }
            else if (_evt.IsClosedEvent || _evt.IsOffEvent)
            {

                DateTime LightActivatedTime =
                    _hub.StateBag.ContainsKey(timeActivatedKey) ? (DateTime)_hub.StateBag[timeActivatedKey] : DateTime.Now;
                timeToWait = DeactivationWait - (DateTime.Now - LightActivatedTime);
            }

            if (EnableDeactivation && SwitchesToActivate.IsAnyOn())
            {
                Console.WriteLine($"{DateTime.Now} {this.GetType().ToString()} is turning off lights in {timeToWait.ToString()}");
                await WaitForCancellationAsync(timeToWait);
                foreach (var s in SwitchesToActivate)
                {
                    await s.Off();
                }
                _hub.StateBag.Remove(timeActivatedKey, out var ignoreMe);
            }
        }
    }
}
