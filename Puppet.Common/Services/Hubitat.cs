using Puppet.Common.Exceptions;
using Puppet.Common.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using Puppet.Common.Configuration;

namespace Puppet.Common.Services
{
    public class Hubitat : HomeAutomationPlatform
    {
        string _baseAddress;
        string _accessToken;
        HttpClient _client;

        public Hubitat()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var hubitatOptions = configuration.GetSection("Hubitat").Get<HubitatOptions>();
            
            _baseAddress = hubitatOptions.BaseUrl;
            _accessToken = hubitatOptions.AccessToken;

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
