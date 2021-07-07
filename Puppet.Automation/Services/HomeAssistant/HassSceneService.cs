
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace Puppet.Automation.Services.HomeAssistant
{
    public class HassSceneService
    {
        Uri _endpoint;
        string _authToken;
        public HassSceneService(IConfiguration config)
        {
            var hassConfig = config.GetSection("HomeAssistant");
            _endpoint = new Uri($"{hassConfig["IntegrationEndpoint"]}/services/scene/turn_on");
            _authToken = hassConfig["Token"];
        }

        public async Task ApplyScene(Scene scene)
        {
            Console.WriteLine($"HASS Scene Service: Activation {scene.ToString()}");
            string bodyText = JsonConvert.SerializeObject(new { entity_id = $"scene.{scene.ToString().ToLower()}" });
            HttpContent contentPost = new StringContent(bodyText, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await client.PostAsync(_endpoint, contentPost);
                response.EnsureSuccessStatusCode();
            }
        }
    }



    public enum Scene
    {
        Warm_Front_Lights,
        Colorful_Front_Lights,
        Arcade_0,
        Arcade_1,
        Arcade_2,
        Arcade_3,
        Arcade_4
    }
}