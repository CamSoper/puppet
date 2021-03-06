using Puppet.Common.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Executive.Interfaces
{
    public interface IExecutive
    {
        public void ProcessEvent(HubEvent evt);
    }
}
