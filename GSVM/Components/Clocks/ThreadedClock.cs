using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSVM.Components.Clocks
{
    public class ThreadedClock
    {
        Thread clockThread = new Thread(new ParameterizedThreadStart(ClockTick));

        CPU cpu;

        public static event EventHandler CPUTicked;

        public static bool ClockEnabled { get; private set; }

        public ThreadedClock(CPU cpu)
        {
            this.cpu = cpu;
        }

        public void Start()
        {
            if (!cpu.Debug)
            {
                clockThread.Start(cpu);
                ClockEnabled = true;
            }
        }

        public void Stop()
        {
            ClockEnabled = false;
        }

        public static void ClockTick(object cpu)
        {
            CPU _cpu = (CPU)cpu;

            while (_cpu.Enabled & ClockEnabled)
            {
                if (!_cpu.Busy)
                {
                    _cpu.Tick();
                    CPUTicked?.Invoke(null, new EventArgs());
                }
            }
        }
    }
}
