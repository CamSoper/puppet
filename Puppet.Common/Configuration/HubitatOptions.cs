using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Configuration
{
    public class HubitatOptions
    {
        public string HubitatHostNameOrIp { get; set; }
        public int MakerApiAppId { get; set; }
        public string AccessToken { get; set; }
        public int AuxAppId { get; set; }
        public string AuxAppAccessToken { get; set; }
    }
}
