using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using Puppet.Common.Notifiers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Automation.Services.Notifiers
{
    class HassAppNotifier : INotifier
    {
        Uri _endpoint;
        string _authToken;

        public string[] Targets { get; set; }

        public HassAppNotifier(IConfiguration config)
        {
            var hassConfig = config.GetSection("HomeAssistant");
            _endpoint = new Uri($"{hassConfig["IntegrationEndpoint"]}/services/notify/notify");
            _authToken = hassConfig["Token"];
        }

        public async Task SendNotification(string message)
        {
            string bodyText = JsonConvert.SerializeObject(new { message = $"{message}" });
            HttpContent contentPost = new StringContent(bodyText, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await client.PostAsync(_endpoint, contentPost);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
