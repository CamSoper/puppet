using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.Caching;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Puppet.Common.Configuration;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Exceptions;
using Puppet.Common.Models;

namespace Puppet.Common.Services
{
    /// <summary>
    /// Represents a physical Hubitat device.
    /// </summary>
    public class Hubitat : HomeAutomationPlatform
    {
        readonly string _baseMakerApiAddress;
        readonly string _accessToken;
        readonly string _baseAuxAppAddress;
        readonly string _auxAppAccessToken;
        readonly string _websocketUrl;
        readonly HttpClient _client;
        readonly MemoryCache _cache;

        SunriseAndSunset _sunriseAndSunset;

        public Hubitat(IConfiguration configuration, HttpClient httpClient)
            : base(configuration)
        {
            HubitatOptions hubitatOptions = configuration.GetSection("Hubitat").Get<HubitatOptions>();

            _baseMakerApiAddress = $"https://{hubitatOptions.HubitatHostNameOrIp}/apps/api/{hubitatOptions.MakerApiAppId}/devices";
            _accessToken = hubitatOptions.AccessToken;
            _websocketUrl = $"wss://{hubitatOptions.HubitatHostNameOrIp}/eventsocket";
            _client = httpClient;
            _cache = MemoryCache.Default;

            _baseAuxAppAddress = $"https://{hubitatOptions.HubitatHostNameOrIp}/apps/api/{hubitatOptions.AuxAppId}";
            _auxAppAccessToken = hubitatOptions.AuxAppAccessToken;

            StateBag = new ConcurrentDictionary<string, object>();
        }

        public override async Task<Dictionary<string, string>> GetDeviceState(IDevice device)
        {
            Uri requestUri = new Uri($"{_baseMakerApiAddress}/{device.Id}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device State Request: {requestUri.ToString().Split('?')[0]}");
            using (var result = await _client.GetAsync(requestUri))
            {
                result.EnsureSuccessStatusCode();

                Dictionary<string, string> state = new Dictionary<string, string>();
                dynamic rawJson = JObject.Parse(await result.Content.ReadAsStringAsync());

                state.Add("name", rawJson.name.ToString());
                state.Add("label", rawJson.label.ToString());
                foreach (dynamic attribute in rawJson.attributes)
                {
                    string key = attribute.name.ToString();
                    if (!state.ContainsKey(key))
                    {
                        state.Add(key, attribute.currentValue.ToString());
                    }
                }

                return state;
            }
        }

        public override async Task DoAction(IDevice device, string action, string[] args = null)
        {
            string secondary = (args != null) ? $"/{args[0]}" : "";
            secondary = secondary.Replace(' ', '-').Replace('?', '.');

            Uri requestUri = new Uri($"{_baseMakerApiAddress}/{device.Id}/{action.Trim()}{secondary.Trim()}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
            using (HttpResponseMessage result = await _client.GetAsync(requestUri))
            {
                result.EnsureSuccessStatusCode();
            }
        }

        public override async Task<T> GetDeviceByLabel<T>(string label)
        {
            var hubitatDevices = await GetHubitatDeviceList();
            string deviceId = hubitatDevices.Where(x => x.Label == label).FirstOrDefault()?.Id;
            if (deviceId == null)
            {
                throw new DeviceNotFoundException($"No device was found with the label: \"{label}\"");
            }
            return await GetDeviceById<T>(deviceId);
        }

        private async Task<List<HubitatDevice>> GetHubitatDeviceList(bool forceRefresh = false)
        {
            string hubitatDeviceListKey = "hubitat-device-list";
            if(_cache.Contains(hubitatDeviceListKey))
            {
                return _cache.Get(hubitatDeviceListKey) as List<HubitatDevice>;
            }

            Uri requestUri = new Uri($"{_baseMakerApiAddress}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");

            using (HttpResponseMessage result = await _client.GetAsync(requestUri))
            {
                result.EnsureSuccessStatusCode();
                List<HubitatDevice> hubitatDevices = JsonConvert.DeserializeObject<List<HubitatDevice>>(await result.Content.ReadAsStringAsync());
                _cache.Add(hubitatDeviceListKey, hubitatDevices, DateTimeOffset.Now.AddMinutes(1));
                return hubitatDevices;
            }
        }
        protected override async Task AuxEndpointNotification(string notificationText, bool playAudio)
        {
            string endpoint = (playAudio) ? "announcement" : "notify";
            Uri requestUri = new Uri($"{_baseAuxAppAddress}/{endpoint}?access_token={_auxAppAccessToken}");

            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
            string jsonText = $"{{ \"notificationText\" : \"{notificationText}\" }}";
            var request = new StringContent(jsonText, Encoding.UTF8, "application/json");
            using (HttpResponseMessage result = await _client.PostAsync(requestUri, request))
            {
                result.EnsureSuccessStatusCode();
            }
        }

        public override async Task<SunriseAndSunset> GetSunriseAndSunset()
        {
            if (_sunriseAndSunset == null ||
                (_sunriseAndSunset?.Sunrise.Date < DateTime.Now.Date))
            {
                Uri requestUri = new Uri($"{_baseAuxAppAddress}/suntimes?access_token={_auxAppAccessToken}");
                Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
                using (HttpResponseMessage result = await _client.GetAsync(requestUri))
                {
                    result.EnsureSuccessStatusCode();
                    _sunriseAndSunset = JsonConvert.DeserializeObject<SunriseAndSunset>(await result.Content.ReadAsStringAsync());
                }
            }
            return _sunriseAndSunset;
        }

    }
}
