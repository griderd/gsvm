using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Processors.CPU_1.Assembler
{
    public partial class Assembler
    {
        string[] registers = Enum.GetNames(typeof(Register));

        bool IsRegister(string operand)
        {
            return registers.Contains(operand.ToUpper());
        }

        bool ExpectRegister(string operand, out uint16_t result)
        {
            result = Int.UInt16;

            if (IsRegister(operand))
            {
                Register register;
                bool isRegister = Enum.TryParse<Register>(operand.ToUpper(), out register);
                result = new uint16_t((ushort)register);
                return isRegister;
            }
            else
            {
                return false;
            }
        }

        bool ExpectLiteral(string operand, out uint16_t result)
        {
            ushort literal;
            bool isLiteral = ushort.TryParse(operand, out literal);
            result = literal;
            return isLiteral;
        }

        bool ExpectSymbol(string operand, out uint16_t result)
        {
            result = Int.UInt16;
            if (symbols.ContainsKey(operand))
            {
                IDataType symbol = symbols[operand];
                if (symbol.Address > ushort.MaxValue)
                {
                    RaiseError("32-bit address support not available.");
                }
                else
                {
                    result = new uint16_t((ushort)symbol.Address);
                }

                return true;
            }
            else
            {
                if ((!IsRegister(operand)) & (operand != ""))
                {
                    //unresolvedSymbols.Add(operand, opcodes.Count);
                    unresolvedSymbols.Add(new UnresolvedSymbol { symbol = operand, location = opcodes.Count });
                    return true;
                }

                return false;
            }
        }
    }
}
