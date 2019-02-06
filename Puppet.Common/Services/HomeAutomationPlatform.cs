using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Puppet.Common.Devices;
using Puppet.Common.Events;

namespace Puppet.Common.Services
{
    /// <summary>
    /// Base class for Home Automation Platforms.
    /// </summary>
    public abstract class HomeAutomationPlatform
    {
        const string _deviceMapFileName = "devicemap.json";
        dynamic _deviceMap { get; }

        public ConcurrentDictionary<string, object> StateBag { get; set; }
        public abstract void DoAction(IDevice device, string action, string[] args = null);
        public abstract Task StartAutomationEventWatcher();

        public event EventHandler<AutomationEventEventArgs> AutomationEvent;

        public HomeAutomationPlatform(IConfiguration configuration)
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
            return ParseAndLookupMappedDeviceName(this._deviceMap, mappedDeviceName);
        }
        string ParseAndLookupMappedDeviceName(dynamic obj, string mappedDeviceName)
        {
            string[] tokens = mappedDeviceName.Split('.');
            if (tokens.Length > 1)
                return ParseAndLookupMappedDeviceName(obj[tokens[0]],
                    mappedDeviceName.Substring(mappedDeviceName.IndexOf(tokens[1])));
            else
                return obj[mappedDeviceName];
        }

        public IDevice GetDeviceByName<T>(string mappedDeviceName)
        {
            return GetDeviceById<T>(LookupDeviceId(mappedDeviceName)) as IDevice;
        }

        public IDevice GetDeviceById<T>(string deviceId)
        {
            return Activator.CreateInstance(typeof(T), new Object[] { this, deviceId }) as IDevice;
        }

        public abstract Dictionary<string, string> GetDeviceState(IDevice device);
    }
}
