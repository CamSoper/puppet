using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Puppet.Common.Devices;
using Puppet.Common.Events;

namespace Puppet.Common.Services
{
    public abstract class HomeAutomationPlatform
    {
        const string _deviceMapFileName = "devicemap.json";
        dynamic _deviceMap { get; }

        public ConcurrentDictionary<string, object> StateBag { get; set; }
        public abstract void DoAction(IDevice device, string action, string[] args = null);
        

        public event EventHandler<AutomationEventEventArgs> AutomationEvent;

        public HomeAutomationPlatform()
        {
            this._deviceMap = JObject.Parse(
                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), _deviceMapFileName)));
        }
        protected virtual void OnAutomationEvent(AutomationEventEventArgs e)
        {
            AutomationEvent?.Invoke(this, e);
        }

        public string LookupDeviceId(string mappedDeviceName)
        {
            return RecursiveLookupDeviceId(this._deviceMap, mappedDeviceName);
        }
        string RecursiveLookupDeviceId(dynamic obj, string mappedDeviceName)
        {
            string[] tokens = mappedDeviceName.Split('.');
            if (tokens.Length > 1)
                return RecursiveLookupDeviceId(obj[tokens[0]], mappedDeviceName.Substring(mappedDeviceName.IndexOf(tokens[1])));
            else
                return obj[mappedDeviceName];
        }

        public IDevice GetDevice<T>(string mappedDeviceName)
        {
            return this.GetDeviceById<T>(LookupDeviceId(mappedDeviceName)) as IDevice;
        }

        public IDevice GetDeviceById<T>(string deviceId)
        {
            return Activator.CreateInstance(typeof(T), new Object[] { this, deviceId }) as IDevice;
        }
    }
}
