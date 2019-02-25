using System;
using System.Threading.Tasks;
using Puppet.Common.Events;

namespace Puppet.Common.Services
{
    public interface IMqttService
    {
        Task SendEventToMqttAsync(HubEvent evt);
        Task Start();
    }
}
