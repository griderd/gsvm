using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Clocks
{
    public class DebugStepping
    {
        public static event EventHandler CPUTicked;

        public static void Step(CPU cpu)
        {
            if (cpu.Enabled)
            {
                if (!cpu.Busy)
                {
                    cpu.Tick();
                    CPUTicked?.Invoke(null, new EventArgs());
                }
            }
        }
    }
}
