using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
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
        SunriseAndSunset _sunriseAndSunset;

        public Hubitat(IConfiguration configuration, HttpClient httpClient) : base(configuration)
        {
            HubitatOptions hubitatOptions = configuration.GetSection("Hubitat").Get<HubitatOptions>();

            _baseMakerApiAddress = $"https://{hubitatOptions.HubitatHostNameOrIp}/apps/api/{hubitatOptions.MakerApiAppId}/devices";
            _accessToken = hubitatOptions.AccessToken;
            _websocketUrl = $"wss://{hubitatOptions.HubitatHostNameOrIp}/eventsocket";
            _client = httpClient;

            _baseAuxAppAddress = $"https://{hubitatOptions.HubitatHostNameOrIp}/apps/api/{hubitatOptions.AuxAppId}"; 
            _auxAppAccessToken = hubitatOptions.AuxAppAccessToken;

            StateBag = new ConcurrentDictionary<string, object>();
        }

        public override Task StartAutomationEventWatcher() => HubitatEventWatcherThread();

        async Task HubitatEventWatcherThread()
        {
            int wseRetryCount = 0;
            while (true)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now} Connecting to Hubitat...");
                    var client = new ClientWebSocket();
                    client.Options.RemoteCertificateValidationCallback += (a, b, c, d) => {
                        return true;
                    };
                    await client.ConnectAsync(new Uri(_websocketUrl), CancellationToken.None);
                    Console.WriteLine($"{DateTime.Now} Websocket success! Watching for events.");
                    wseRetryCount = 0;
                    ArraySegment<byte> buffer;
                    while (client.State == WebSocketState.Open)
                    {
                        buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                        WebSocketReceiveResult reply = await client.ReceiveAsync(buffer, CancellationToken.None);
                        string json = System.Text.Encoding.Default.GetString(buffer.ToArray()).TrimEnd('\0');
                        HubEvent evt = JsonConvert.DeserializeObject<HubEvent>(json);
                        OnAutomationEvent(new AutomationEventEventArgs() { HubEvent = evt });
                    }
                }
                catch (WebSocketException wse)
                {
                    wseRetryCount++;
                    int waitTimeInSecs = (wseRetryCount > 5) ? 150 : 5;
                    Console.WriteLine($"{DateTime.Now} Hubitat websocket error! {wse.Message} -- Retrying in {waitTimeInSecs} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(waitTimeInSecs));
                }
                catch (UriFormatException ufe)
                {
                    Console.WriteLine($"{DateTime.Now} URI Format Exception! Fix your config! {ufe.Message}");
                    await Task.Delay(Timeout.InfiniteTimeSpan);
                }
                catch (Exception ex)
                {
                    // Something unknown went wrong. I don't care what 
                    // because this method should run forever. Just mention it.
                    Console.WriteLine($"{DateTime.Now} {ex} {ex.Message}");
                }
            }
        }

        public override Dictionary<string, string> GetDeviceState(IDevice device)
        {
            Uri requestUri = new Uri($"{_baseMakerApiAddress}/{device.Id}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device State Request: {requestUri.ToString().Split('?')[0]}");
            var result = _client.GetAsync(requestUri).Result;
            result.EnsureSuccessStatusCode();

            Dictionary<string, string> state = new Dictionary<string, string>();
            dynamic rawJson = JObject.Parse(result.Content.ReadAsStringAsync().Result);

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

        public override void DoAction(IDevice device, string action, string[] args = null)
        {
            string secondary = (args != null) ? $"/{args[0]}" : "";
            secondary = secondary.Replace(' ', '-').Replace('?', '.');

            Uri requestUri = new Uri($"{_baseMakerApiAddress}/{device.Id}/{action.Trim()}{secondary.Trim()}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
            HttpResponseMessage result = _client.GetAsync(requestUri).Result;
            result.EnsureSuccessStatusCode();
        }

        public override IDevice GetDeviceByLabel<T>(string label)
        {
            Uri requestUri = new Uri($"{_baseMakerApiAddress}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
            HttpResponseMessage result = _client.GetAsync(requestUri).Result;
            result.EnsureSuccessStatusCode();

            List<HubitatDevice> hubitatDevices = JsonConvert.DeserializeObject<List<HubitatDevice>>(result.Content.ReadAsStringAsync().Result);
            string deviceId = hubitatDevices.Where(x => x.Label == label).FirstOrDefault()?.Id;
            if(deviceId == null)
            {
                throw new DeviceNotFoundException("No device was found with the provided label.");
            }
            return this.GetDeviceById<GenericDevice>(deviceId);
        }

        public override async Task SendNotification(string notificationText)
        {
            throw new NotImplementedException();
        }
        
        public override SunriseAndSunset SunriseAndSunset
        {
            get
            {
                if(_sunriseAndSunset == null || 
                    (_sunriseAndSunset?.Sunrise.Date < DateTime.Now.Date))
                {
                    Uri requestUri = new Uri($"{_baseAuxAppAddress}/suntimes?access_token={_auxAppAccessToken}");
                    Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
                    HttpResponseMessage result = _client.GetAsync(requestUri).Result;
                    result.EnsureSuccessStatusCode();
                    _sunriseAndSunset = JsonConvert.DeserializeObject<SunriseAndSunset>(result.Content.ReadAsStringAsync().Result);
                }
                return _sunriseAndSunset;    
            }
        }

    }
}
