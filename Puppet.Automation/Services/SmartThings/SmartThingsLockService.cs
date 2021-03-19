
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Puppet.Automation.Services.SmartThings
{
    public class SmartThingsLockService
    {
        Uri _endpoint;
        string _authToken;
        public SmartThingsLockService(IConfiguration config)
        {
            var lockConfig = config.GetSection("FrontDoorLock");
            _endpoint = new Uri(lockConfig["SmartThingsIntegrationEndpoint"]);
            _authToken = lockConfig["SmartThingsToken"];
        }

        public async Task Lock()
        {
            await Send(Action.Lock);
        }

        
        public async Task Unlock()
        {
            await Send(Action.Unlock);
        }

        
        public async Task Refresh()
        {
            await Send(Action.Refresh);
        }

        private async Task Send(Action action)
        {
            string bodyText = JsonConvert.SerializeObject(new { lockAction = $"{action.ToString().ToLower()}" });
            HttpContent contentPost = new StringContent(bodyText, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await client.PostAsync(_endpoint, contentPost);
                response.EnsureSuccessStatusCode();
            }
        }

        enum Action
        {
            Lock,
            Unlock,
            Refresh            
        }
    }
}