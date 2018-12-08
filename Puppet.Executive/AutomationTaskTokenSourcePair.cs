using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Puppet.Executive
{
    public class AutomationTaskTokenSourcePair
    {
        public AutomationTask Task { get; set; }
        public CancellationTokenSource CTS { get; set; }

        public AutomationTaskTokenSourcePair(AutomationTask task, CancellationTokenSource cts)
        {
            Task = task;
            CTS = cts;
        }
    }
}
