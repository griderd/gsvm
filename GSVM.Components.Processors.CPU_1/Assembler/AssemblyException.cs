using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Processors.CPU_1.Assembler
{
    public class AssemblyException : Exception
    {
        public string Code { get; private set; }
        public int LineNumber { get; private set; }

        public AssemblyException(string message, string code, int lineNumber)
            : base(message)
        {
            Code = code;
            LineNumber = lineNumber;
        }
    }
}
