using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Processors.CPU_1
{
    [Flags]
    public enum CPUFlags : ulong
    {
        Empty = 0,
        
        WaitForInterrupt = 1,
        ArithmeticOverflow = 2,
        Equal = 4,
        GreaterThan = 8,
        LessThan = 16,
        MemoryError = 32
    }
}
