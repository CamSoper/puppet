using System;
using System.Collections.Generic;
using System.Text;

namespace Puppet.Common.Events
{
    public static class ExtensionMethods
    {
        public static bool IsOpenEvent(this HubEvent evt)
        {
            if(evt.value == "open")
            {
                return true;
            }

            return false;
        }

        public static bool IsClosedEvent(this HubEvent evt)
        {
            if (evt.value == "closed")
            {
                return true;
            }

            return false;
        }

        public static bool IsLockedEvent(this HubEvent evt)
        {
            if (evt.value == "locked")
            {
                return true;
            }

            return false;
        }

        public static bool IsUnLockedEvent(this HubEvent evt)
        {
            if (evt.value == "unlocked")
            {
                return true;
            }

            return false;
        }

    }
}
