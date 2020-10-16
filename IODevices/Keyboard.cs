using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Devices;
using GSVM.Peripherals.IODevices.WindowsInterop;

namespace GSVM.Peripherals.IODevices
{
    public class Keyboard : GenericSerialBus
    {
        public Keyboard()
        {
            KeyboardIntercept.Init();
            KeyboardIntercept.KeyboardEvent += KeyboardIntercept_KeyboardEvent;
            deviceType = 3;
            GenerateID();
        }

        private void KeyboardIntercept_KeyboardEvent(object sender, KeyboardEventArgs e)
        {
            GenericSerialRequest data = new GenericSerialRequest(0, (uint)e.KeyCode);
            ReadData = data;
            Interrupt = true;
            ReadyToRead = true;
        }

        public override void Dispose()
        {
            KeyboardIntercept.Deinit();
            base.Dispose();
        }

        public override void ClockTick()
        {
            base.ClockTick();
        }
    }
}
