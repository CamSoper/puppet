using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Executive
{
    public class AutomationTask : Task
    {
        public Type AutomationType { get; }
        public AutomationTask(Action action, CancellationToken token, Type automationType) : base(action, token)
        {
            this.AutomationType = automationType;
        }
    }
}
