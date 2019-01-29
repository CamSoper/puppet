using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Events
{
    public class AutomationEventEventArgs : EventArgs
    {
        public HubEvent HubEvent { get; set; }
    }
}
