using Puppet.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Common.Automation
{
    public interface IAutomation
    {
        /// <summary>
        /// Handles events coming from the home automation controller.
        /// </summary>
        /// <param name="token">A .NET cancellation token received if this handler is to be cancelled.</param>
        Task Handle(CancellationToken token);
    }
}
