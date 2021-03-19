using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Notifiers
{
    public interface INotifier
    {
        Task SendNotification(string message);
    }
}
