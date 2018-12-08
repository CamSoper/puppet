using Puppet.Common.Exceptions;
using Puppet.Common.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Puppet.Common.Services
{
    public class Hubitat : HomeAutomationPlatform
    {
        string _baseAddress;
        string _accessToken;
        HttpClient _client;

        public Hubitat()
        {
            // TODO: Move to configuration
            _baseAddress = "http://192.168.15.137/apps/api/143/devices";
            _accessToken = "133adfd3-675c-4ca6-a7cb-7220aa4e75b4";

            _client = new HttpClient();

            StateBag = new ConcurrentDictionary<string, object>();
        }

        public override void DoAction(IDevice device, string action, string[] args = null)
        {
            // http://[IP Address]/apps/api/143/devices/[Device ID]/[Command]/[Secondary value]?access_token=

            string secondary = (args != null) ? $"/{args[0]}" : "";
            secondary = secondary.Replace(' ', '-').Replace('?', '.');

            Uri requestUri = new Uri($"{_baseAddress}/{device.Id}/{action.Trim()}{secondary.Trim()}?access_token={_accessToken}");
            var result = _client.GetAsync(requestUri).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new AutomationHubException();
            }
        }

        public override IDevice GetDevice()
        {
            throw new NotImplementedException();
        }

    }
}
