using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPad
{

    public class ButtonMapping
    {
        public byte inputReportIndex;
        public byte inputReportMask;
        public string button;
    }

    public class GamepadMapping
    {
        public string GamepadName;
        public ButtonMapping[] buttonMapping;
    }

    public class KeypadMapping
    {
        public string targetWindowName;
        public Dictionary<string, Keyboard.Messaging.VKeys> keymapping = new Dictionary<string, Keyboard.Messaging.VKeys>();
    }
}
