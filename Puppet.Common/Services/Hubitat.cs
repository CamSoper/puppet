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

namespace Puppet.Common.Services
{
    /// <summary>
    /// Represents a physical Hubitat device.
    /// </summary>
    public class Hubitat : HomeAutomationPlatform
    {
        readonly string _baseAddress;
        readonly string _accessToken;
        readonly string _websocketUrl;
        readonly HttpClient _client;

        public Hubitat(IConfiguration configuration, HttpClient httpClient) : base(configuration)
        {
            HubitatOptions hubitatOptions = configuration.GetSection("Hubitat").Get<HubitatOptions>();

            _baseAddress = hubitatOptions.BaseUrl;
            _accessToken = hubitatOptions.AccessToken;
            _websocketUrl = hubitatOptions.EventWebsocketUrl;
            _client = httpClient;

            StateBag = new ConcurrentDictionary<string, object>();
        }

        public override Task StartAutomationEventWatcher() => HubitatEventWatcherThread();

        async Task HubitatEventWatcherThread()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now} Connecting to Hubitat...");
                    var client = new ClientWebSocket();
                    await client.ConnectAsync(new Uri(_websocketUrl), CancellationToken.None);
                    Console.WriteLine($"{DateTime.Now} Websocket success! Watching for events.");
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
                catch (System.Net.WebSockets.WebSocketException wse)
                {
                    Console.WriteLine($"{DateTime.Now} Hubitat websocket error! {wse.Message} -- Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (System.UriFormatException ufe)
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

        public override Dictionary<string, string> GetDeviceProperties(IDevice device)
        {
            Uri requestUri = new Uri($"{_baseAddress}/{device.Id}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Information Request: {requestUri.ToString().Split('?')[0]}");
            var result = _client.GetAsync(requestUri).Result;
            result.EnsureSuccessStatusCode();

            Dictionary<string, string> properties = new Dictionary<string, string>();
            dynamic rawProperties = JObject.Parse(result.Content.ReadAsStringAsync().Result);

            properties.Add("name", rawProperties.name.ToString());
            properties.Add("label", rawProperties.label.ToString());
            foreach (dynamic attribute in rawProperties.attributes)
            {
                properties.Add(attribute.name.ToString(), attribute.currentValue.ToString());
            }

            return properties;
        }

        public override void DoAction(IDevice device, string action, string[] args = null)
        {
            string secondary = (args != null) ? $"/{args[0]}" : "";
            secondary = secondary.Replace(' ', '-').Replace('?', '.');

            Uri requestUri = new Uri($"{_baseAddress}/{device.Id}/{action.Trim()}{secondary.Trim()}?access_token={_accessToken}");
            Console.WriteLine($"{DateTime.Now} Hubitat Device Command: {requestUri.ToString().Split('?')[0]}");
            var result = _client.GetAsync(requestUri).Result;
            result.EnsureSuccessStatusCode();
        }
    }
}
