using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Devices
{
    public class Speaker : IDevice
    {
        HomeAutomationPlatform _hub;
        public string Id { get; }

        public string Name => throw new NotImplementedException();

        public string Label => throw new NotImplementedException();

        public Speaker(HomeAutomationPlatform hub, string id)
        {
            _hub = hub;
            this.Id = id; 
        }

        public void Speak(string message)
        {
            Console.WriteLine($"Speaker ID {Id} speaking: {message}");
            _hub.DoAction(this, "speak", new string[] { message });
        }
    }
}
