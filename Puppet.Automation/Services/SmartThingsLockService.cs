
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Puppet.Automation.Services
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
            await Send(JsonConvert.SerializeObject(new { lockAction = "lock" }));
        }

        
        public async Task Unlock()
        {
            await Send(JsonConvert.SerializeObject(new { lockAction = "unlock" }));
        }

        
        public async Task Refresh()
        {
            await Send(JsonConvert.SerializeObject(new { lockAction = "refresh" }));
        }

        private async Task Send(string bodyText)
        {
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