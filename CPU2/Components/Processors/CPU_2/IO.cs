using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSVM.Constructs.FastDataTypes;
using GSVM.Components.Processors.CPU_2;

namespace GSVM.Components.Processors
{
    public partial class CPU2 : CPU
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
            uint32_t buffer = registers.Read<uint32_t>(Register.ECX);

            Northbridge.ReadFromPort(port, address, buffer);
        }

        void intr(Register_t r)
        {
            uint16_t value = registers.Read<uint16_t>(r);
            Interrupt(value.Value);
        }

        void intl(uint16_t l)
        {
            Interrupt(l.Value);
        }
    }
}
