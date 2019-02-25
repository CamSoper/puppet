using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Exceptions
{
    public class DeviceNotFoundException : System.Exception
    {
        public DeviceNotFoundException(string message) : base(message){}
    }
}
