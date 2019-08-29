using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSVM.Components.Controllers;

namespace GSVM.Components.Clocks
{
    public class ThreadedClock : ClockGenerator
    {
        Thread clockThread = new Thread(new ParameterizedThreadStart(ClockTick));

        public ThreadedClock(Northbridge northbridge)
            : base(northbridge)
        {
            
        }

        public override void Start()
        {
            if (!northbridge.CPU.Debug)
            {
                ThreadState state = clockThread.ThreadState;
                clockThread.Start(northbridge);
                ClockEnabled = true;
            }
        }

        public override void Stop()
        {
            ClockEnabled = false;
        }

        public static void ClockTick(object nb)
        {
            Northbridge _nb = (Northbridge)nb;

            while (_nb.CPU.Enabled & ClockEnabled)
            {
                if (!_nb.CPU.Busy)
                {
                    _nb.ClockTick();
                    RaiseTick();
                }
            }
        }
    }
}
