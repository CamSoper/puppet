using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Puppet.Executive
{
    /// <summary>
    /// This is a composite model for holding AutomationTasks
    /// and their matching CancellationTokenSources.
    /// </summary>
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
