using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Processors.CPU_1;
using GSVM.Constructs.DataTypes;
using GSVM.Components.Clocks;
using GSVM.Constructs;

namespace GSVM.Components.Processors
{
    public partial class CPU1 : CPU
    {
        void _out()
        {
            uint32_t port = registers.Read<uint32_t>(Register.EAX);
            uint32_t address = registers.Read<uint32_t>(Register.EBX);
            uint32_t length = registers.Read<uint32_t>(Register.ECX);

            Northbridge.WriteToPort(port, address, length);
        }

        void _in()
        {
            uint32_t port = registers.Read<uint32_t>(Register.EAX);
            uint32_t address = registers.Read<uint32_t>(Register.EBX);

            Northbridge.ReadFromPort(port, address);
        }
    }
}
