using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Executive
{
    public class AutomationTask : Task
    {
        public string AutomationName { get; }
        public AutomationTask(Action action, CancellationToken token, string automationName) : base(action, token)
        {
            this.AutomationName = automationName;
        }
    }
}
