using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puppet.Common.Devices
{
    public interface IDevice
    {
        string Id { get; }
        string Name { get; }
        string Label { get; }
    }
}
