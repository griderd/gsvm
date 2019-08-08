using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs;
using GSVM.Components.Controllers;

namespace GSVM.Components
{
    public class MemoryAccessException : Exception
    {
        public SmartPointer Pointer { get; private set; }

        public MemoryAccessException(SmartPointer ptr, bool read)
            : base(string.Format("Cannot {0} memory at address 0x{1:X8}:0x{2:X8} due to invalid access rights."))
        {
            Pointer = ptr;
        }
    }
}
