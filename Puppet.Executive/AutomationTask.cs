using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Executive
{
    public class AutomationTask
    {
        public Type AutomationType { get; }
        private readonly Func<Task> taskFunc;

        public AutomationTask(Func<Task> action, Type automationType) =>
         (AutomationType, taskFunc) = (automationType, action);

         public Task Start() => taskFunc();
    }
}
