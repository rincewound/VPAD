using SharpLib.Hid;
using SharpLib.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VPad
{
    class HIDDevice
    {
        Device controller;
        Handler hidHandler;

        private static RAWINPUTDEVICELIST[] EnumerateDevices(RawInputDeviceType ty)
        {
            RAWINPUTDEVICELIST[] retVal = null;
            uint count = 0;
            int res = Function.GetRawInputDeviceList(retVal, ref count, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            retVal = new RAWINPUTDEVICELIST[count];
            res = Function.GetRawInputDeviceList(retVal, ref count, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            return retVal.Where(x => x.dwType == ty).ToArray();
        }

        public HIDDevice(IntPtr wndHandle, Handler.HidEventHandler hidEventHandler)
        {
            var devices = EnumerateDevices(RawInputDeviceType.RIM_TYPEHID);
            int i = 0;
            do
            {
                controller = new Device(devices[i].hDevice);
                i++;
                if (i > devices.Length)
                {
                    throw new Exception("No gamepad connected.");
                }
            } while (!controller.IsGamePad);

            var rid = new RAWINPUTDEVICE[1];
            rid[0] = new RAWINPUTDEVICE();
            rid[0].usUsagePage = controller.UsagePage;
            rid[0].usUsage = controller.UsageCollection;
            rid[0].dwFlags = RawInputDeviceFlags.RIDEV_EXINPUTSINK;
            rid[0].hwndTarget = wndHandle;

            hidHandler = new Handler(rid, true);
            hidHandler.OnHidEvent += hidEventHandler;
        }

        public void InjectEvent(ref System.Windows.Forms.Message msg)
        {
            hidHandler.ProcessInput(ref msg);
        }

    }
}
