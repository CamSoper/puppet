using Puppet.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Puppet.Common.Models.Automation
{
    public interface IAutomation
    {
        /// <summary>
        /// Handles events coming from the home automation controller.
        /// </summary>
        /// <param name="evt">The event passed from the automation controller.</param>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        void Handle(HubEvent evt, CancellationToken token);
    }
}
