using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Compiler
{
    enum ConditionalState
    {
        None,
        IfDef,
        IfNDef,
        Else,
        ElIf
    }
}
