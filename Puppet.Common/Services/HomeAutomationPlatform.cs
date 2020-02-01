using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Models;
using Puppet.Common.Telemetry;

namespace Puppet.Common.Services
{
    /// <summary>
    /// Base class for Home Automation Platforms.
    /// </summary>
    public abstract class HomeAutomationPlatform
    {
        const string DEVICE_FILENAME = "devicemap.json";

        JsonElement DeviceMap { get; }

        public TelemetryClient TelemetryClient { get; }
        public ConcurrentDictionary<string, object> StateBag { get; set; }
        public IConfiguration Configuration { get; set; }
        public abstract Task DoAction(IDevice device, string action, string[] args = null);
        public abstract Task SendNotification(string notificationText, bool audioAnnouncement);
        
        public abstract Task StartAutomationEventWatcher();

        public event EventHandler<AutomationEventEventArgs> AutomationEvent;

        public HomeAutomationPlatform(IConfiguration configuration)
        {
            this.DeviceMap = JsonDocument
                                .Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), DEVICE_FILENAME)))
                                .RootElement;

            Configuration = configuration;

            TelemetryClient = AppInsights.GetTelemetryClient(configuration);
        }

        public abstract Task<SunriseAndSunset> GetSunriseAndSunset();

        protected virtual void OnAutomationEvent(AutomationEventEventArgs e)
        {
            AutomationEvent?.Invoke(this, e);
        }

        public string LookupDeviceId(string mappedDeviceName)
        {
            return ParseAndLookupMappedDeviceName(this.DeviceMap, mappedDeviceName);
        }
        string ParseAndLookupMappedDeviceName(JsonElement map, string mappedDeviceName)
        {
            string[] tokens = mappedDeviceName.Split('.');
            JsonElement deviceElement = map.Clone();
            if (tokens.Length > 1)
            { 
                for (int i = 0; i < tokens.Length; i++)
                {
                    deviceElement = deviceElement.GetProperty(tokens[i]);
                }
            }

            return deviceElement.GetString();
        }

        public async Task<T> GetDeviceByMappedName<T>(string mappedDeviceName)
        {
            return await GetDeviceById<T>(LookupDeviceId(mappedDeviceName));
        }

        public async Task<T> GetDeviceById<T>(string deviceId)
        {
            return await Task.FromResult((T)Activator.CreateInstance(typeof(T), new Object[] { this, deviceId }));
        }

        public abstract Task<Dictionary<string, string>> GetDeviceState(IDevice device);

        public abstract Task<T> GetDeviceByLabel<T>(string label);
    }
}
