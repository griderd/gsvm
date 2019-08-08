using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Controllers;

namespace GSVM.Components.Clocks
{
    public class DebugStepping : ClockGenerator
    {
        public DebugStepping(Northbridge northbridge) : base(northbridge)
        {
        }

        public static void Step(Northbridge nb)
        {
            if (nb.CPU.Enabled)
            {
                if (!nb.CPU.Busy)
                {
                    nb.ClockTick();
                    RaiseTick();
                }
            }
        }
    }
}
