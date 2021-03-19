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
    class HassAlexaNotifier : INotifier
    {
        Uri _endpoint;
        string _authToken;
        string[] _targets;

        public HassAlexaNotifier(IConfiguration config, string[] targets)
        {
            var hassConfig = config.GetSection("HomeAssistant");
            _endpoint = new Uri($"{hassConfig["IntegrationEndpoint"]}/services/notify/alexa_media");
            _authToken = hassConfig["Token"];
            _targets = FullyQualifiedTargetList(targets).ToArray();
        }

        public async Task SendNotification(string message)
        {
            string bodyText = JsonConvert.SerializeObject(
                new {
                    target = _targets,
                    message,
                    data = new { type = "announce", method = "all" }
                });

            HttpContent contentPost = new StringContent(bodyText, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await client.PostAsync(_endpoint, contentPost);
                response.EnsureSuccessStatusCode();
            }

        }

        private IEnumerable<string> FullyQualifiedTargetList(string[] targets)
        {
            foreach (var s in targets)
            {
                yield return $"media_player.{s}";
            }
        }
    }
}
