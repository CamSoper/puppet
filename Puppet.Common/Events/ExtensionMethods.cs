using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppet.Common.Events
{
    public static class ExtensionMethods
    {

        public static ButtonState GetButtonState(this HubEvent evt)
        {
            if (evt.IsButtonPushedEvent)
            {
                return ButtonState.Pushed;
            }
            else if (evt.IsButtonHeldEvent)
            {
                return ButtonState.Held;
            }
            else if (evt.IsButtonDoubleTappedEvent)
            {
                return ButtonState.DoubleTapped;
            }
            else
            {
                return ButtonState.Unknown;
            }
        }
    }
    public enum ButtonState
    {
        Pushed,
        Held,
        DoubleTapped,
        Released,
        Unknown
    }
}
