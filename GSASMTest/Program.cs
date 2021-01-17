using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Assembler;
using GSVM.Assembler.Targets;

namespace GSASMTest
{
    class Program
    {
        static string testCode = @"
; sample comment
jmp main
main:
    hlt";

        static void Main(string[] args)
        {
            Assembler asm = new Assembler(CPU1.Name);

            byte[] data = asm.Assemble(testCode);
        }
    }
}
