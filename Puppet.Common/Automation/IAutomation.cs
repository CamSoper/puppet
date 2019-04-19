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
        Task Handle(CancellationToken token);
    }
}
