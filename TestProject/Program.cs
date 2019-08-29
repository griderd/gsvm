using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM;
using GSVM.Components;
using GSVM.Components.Clocks;
using GSVM.Components.Processors;
using GSVM.Constructs.DataTypes;

namespace TestProject
{
    class Program
    {
        static VirtualMachine vm;

        static byte[] sampleProgram = new byte[]
            {
                0x0020, 0x0004,                 // jump main
                0x0007, 0x000C, 0x0001,         // main: mov al, 1
                0x0007, 0x000D, 0x0002,         // mov ah, 2
                0x0009, 0x000C, 0x000D,         // add al, ah
                0x001E                          // hlt
            };

        static void Main(string[] args)
        {
            //vm = new VirtualMachine(new CPU1(), 16384);

            // Write a halt instruction to the start for the time being
            // The memory is otherwise blank (all zeros) and would continuously operate the idle instruction
            // until it reached the end of memory.

            //vm.memory.Write(0, new uint16_t((ushort)GSVM.Components.Processors.CPU_1.Opcodes.hlt));

            vm.LoadMemory(sampleProgram);
            vm.Start();
        }
    }
}
