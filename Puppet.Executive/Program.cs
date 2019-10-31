using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Puppet.Automation;
using Puppet.Executive.Automation;

namespace Puppet.Executive
{

    class Program
    {
        const string APPSETTINGS_FILENAME = "appsettings.json";

        public static async Task Main(string[] args)
        {
            // Read the configuration file
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                .AddJsonFile(APPSETTINGS_FILENAME, optional: false, reloadOnChange: true)
                .Build();

            var service = new PuppetService(configuration, new AutomationFactory(configuration)); 

            await service.Start();
        }
    }
}
