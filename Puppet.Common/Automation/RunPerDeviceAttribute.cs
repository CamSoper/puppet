using System;

namespace Puppet.Common.Automation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RunPerDeviceAttribute : Attribute
    {
    }

}
