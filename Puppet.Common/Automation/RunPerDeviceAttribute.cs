using System;

namespace Puppet.Common.Automation
{
    /// <summary>
    /// Indicates that this IAutomation should run one instance per device listed as a TriggerDevice. Otherwise, the automation is only allowed to have one instance at any time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RunPerDeviceAttribute : Attribute
    {
    }
}
