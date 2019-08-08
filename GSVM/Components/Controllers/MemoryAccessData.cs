using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Controllers
{
    public class MemoryAccessData
    {
        public int Ring { get; private set; }
        public bool ReadOnly { get; private set; }

        public MemoryAccessData(int ring, bool readOnly)
        {
            Ring = ring;
            ReadOnly = readOnly;
        }
    }
}
