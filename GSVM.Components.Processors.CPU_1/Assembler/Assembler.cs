using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GSVM.Constructs.DataTypes;

namespace GSVM.Components.Processors.CPU_1.Assembler
{
    public partial class Assembler
    {
        enum Stage
        {
            Preparsing,
            Preassembly,
            Postassembly
        }

        int lineNumber = 0;

        Stage stage = Stage.Preparsing;

        string instructionCapture = "([a-zA-Z0-9]{2,})[ ]?([a-zA-Z0-9_]*)(, )?([a-zA-Z0-9_]*)";
        string variableCapture = "([a-zA-Z]{1}[a-zA-Z0-9_]*) ([a-zA-Z_]{1}[a-zA-Z0-9_\\[\\]]*) (\\-?[0-9]{1,}[bh]?|\".*\"|\'.{1}\'|@[a-zA-Z]{1}[a-zA-Z0-9_]*)";
        string arrayCapture = "([a-zA-Z]{1}[a-zA-Z0-9_]*)\\[([0-9]{1,})\\] ([a-zA-Z_]{1}[a-zA-Z0-9_]*) \\{((?:(?:-?[0-9]{1,}[bh]?|\".*\"|\'.{1}\'|@[a-zA-Z]{1}[a-zA-Z0-9_]*),?){1,}|default)\\}";
        string stringCapture = "string ([a-zA-Z]{1}[a-zA-Z0-9_]*) \"(.*)\"";

        List<byte> binary = new List<byte>();
        public byte[] Binary { get { return binary.ToArray(); } }

        Encoding encoding = Encoding.GetEncoding(437);

        ushort address = 0;
        
        List<string> buildList;
        List<Opcode> opcodes;
        Dictionary<string, IDataType> symbols;
        List<string> labels;
        List<UnresolvedSymbol> unresolvedSymbols;

        Dictionary<string, string> pragma;

        List<string> sourceCode;
        public string[] SourceCode { get { return sourceCode.ToArray(); } }

        delegate bool Expect(string ain, out uint16_t aout);

        string CurrentLine { get { return sourceCode[lineNumber - 1].Trim(); } }

        public ushort Offset { get; set; }

        ushort TrueAddress { get { return (ushort)(Offset + address); } }

        public void AddSource(string[] code)
        {
            sourceCode.AddRange(code);
        }

        public Assembler(Dictionary<string, string> pragma = null)
        {
            binary = new List<byte>();
            buildList = new List<string>();
            opcodes = new List<Opcode>();
            symbols = new Dictionary<string, IDataType>();
            labels = new List<string>();
            unresolvedSymbols = new List<UnresolvedSymbol>();
            sourceCode = new List<string>();

            if (pragma == null)
                this.pragma = new Dictionary<string, string>();
            else
                this.pragma = pragma;
        }

        void RaiseError(string message)
        {
            throw new AssemblyException(message, CurrentLine, lineNumber);
        }

        void InvalidOperands()
        {
            try
            {
                RaiseError("Invalid operands");
            }
            catch
            {
                throw;
            }
        }

        public uint Length
        {
            get
            {
                uint length = 0;

                for (int i = 0; i < buildList.Count; i++)
                {
                    string id = buildList[i];

                    if (id.StartsWith("$"))
                    {
                        length += 8;
                    }
                    else if (!labels.Contains(id))
                    {
                        length += symbols[id].Length;
                    }
                }

                return length;
            }
        }

        public void Assemble()
        {
            opcodes.Clear();
            binary.Clear();
            buildList.Clear();
            symbols.Clear();
            labels.Clear();
            unresolvedSymbols.Clear();
            address = 0;
            lineNumber = 0;

            try
            {
                stage = Stage.Preparsing;
                ResolvePragmas();

                for (int i = 0; i < sourceCode.Count; i++)
                {
                    lineNumber = i + 1;
                    Parse(sourceCode[i]);
                }

                stage = Stage.Preassembly;

                ResolveSymbols();
                ResolvePragmas();
            }
            catch
            {
                throw;
            }

            for (int i = 0; i < buildList.Count; i++)
            {
                string id = buildList[i];

                if (id.StartsWith("$"))
                {
                    string _opid = id.Substring(1, id.Length - 1);
                    int opid = int.Parse(_opid);
                    binary.AddRange(opcodes[opid].ToBinary());
                }
                else if (!labels.Contains(id))
                {
                    binary.AddRange(symbols[id].ToBinary());
                }
                
            }

            stage = Stage.Postassembly;
            ResolvePragmas();
        }

        void Parse(string str)
        {
            string line = str.Trim();

            if (line == "")
                return;

            if (line.StartsWith("#") | line.StartsWith(";"))
                return;

            if (line.EndsWith(":"))
            {
                ParseLabel(line.Substring(0, line.Length - 1));
                return;
            }

            if (Regex.IsMatch(line, stringCapture))
            {
                ParseString(line);
            }
            else if (Regex.IsMatch(line, variableCapture))
            {
                ParseVariable(line);
            }
            else if (Regex.IsMatch(line, arrayCapture))
            {
                ParseArray(line);
            }
            else if (Regex.IsMatch(line, instructionCapture))
            {               
                ParseInstruction(line);
            }
            else
            {
                RaiseError("Line could not be parsed.");
            }
        }

        void ParseLabel(string label)
        {
            if (symbols.ContainsKey(label))
                RaiseError(string.Format("Label \"{0}\" already exists.", label));
            else
            {
                symbols.Add(label, new uint16_t((uint)TrueAddress));
                labels.Add(label);
            }
        }

        void ParseVariable(string line)
        {
            Match match = Regex.Match(line, variableCapture);

            string type = match.Groups[1].Value;
            string name = match.Groups[2].Value;
            string value = match.Groups[3].Value;

            switch (type)
            {
                case "uint8":
                case "byte":
                    byte tempu8;
                    if (byte.TryParse(value, out tempu8))
                    {
                        symbols.Add(name, new uint8_t(tempu8, TrueAddress));
                        address += 1;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse uint8 value.");
                    break;

                case "int8":
                case "sbyte":
                    sbyte temp8;
                    if (sbyte.TryParse(value, out temp8))
                    {
                        symbols.Add(name, new int8_t(temp8, TrueAddress));
                        address += 1;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse int8 value.");
                    break;

                case "ushort":
                case "uint16":
                    ushort tempu16;
                    if (ushort.TryParse(value, out tempu16))
                    {
                        symbols.Add(name, new uint16_t(tempu16, TrueAddress));
                        address += 2;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse uint16 value.");
                    break;

                case "ptr":
                    ushort tempptr;
                    string symb = value.Substring(1);
                    if (ushort.TryParse(value, out tempptr))
                    {
                        symbols.Add(name, new uint16_t(tempptr, TrueAddress));
                        address += 2;
                        buildList.Add(name);
                    }
                    else if (symbols.ContainsKey(symb))
                    {
                        uint ptr = symbols[symb].Address;
                        if (ptr > ushort.MaxValue)
                        {
                            RaiseError("Pointer value exceeds uint16 max value.");
                        }
                        else
                        {
                            symbols.Add(name, new uint16_t((ushort)ptr, TrueAddress));
                            address += 2;
                            buildList.Add(name);
                        }
                    }
                    else if (!symbols.ContainsKey(symb))
                        RaiseError(string.Format("Symbol \"{0}\" does not exist before pointer \"{1}\".", value, name));
                    else
                        RaiseError("Cannot parse pointer.");
                    break;

                case "short":
                case "int16":
                    short temp16;
                    if (short.TryParse(value, out temp16))
                    {
                        symbols.Add(name, new int16_t(temp16, TrueAddress));
                        address += 2;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse int16 value.");
                    break;

                case "lptr":
                case "uint":
                case "uint32":
                    uint tempu32;
                    if (uint.TryParse(value, out tempu32))
                    {
                        symbols.Add(name, new uint32_t(tempu32, TrueAddress));
                        address += 4;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse uint32 value.");
                    break;

                case "int":
                case "int32":
                    int temp32;
                    if (int.TryParse(value, out temp32))
                    {
                        symbols.Add(name, new int32_t(temp32, TrueAddress));
                        address += 4;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse int32 value.");
                    break;

                case "ulong":
                case "uint64":
                    ulong tempu64;
                    if (ulong.TryParse(value, out tempu64))
                    {
                        symbols.Add(name, new uint64_t(tempu64, TrueAddress));
                        address += 8;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse uint64 value.");
                    break;

                case "long":
                case "int64":
                    long temp64;
                    if (long.TryParse(value, out temp64))
                    {
                        symbols.Add(name, new int64_t(temp64, TrueAddress));
                        address += 8;
                        buildList.Add(name);
                    }
                    else
                        RaiseError("Cannot parse int64 value.");
                    break;
            }
        }

        void ParseString(string line)
        {
            Match match = Regex.Match(line, stringCapture);

            string name = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            string literalValue = StringEscaper.StringLiteral(value);

            byte[] encoded = encoding.GetBytes(literalValue);           

            int length = literalValue.Length;

            for (int i = 0; i < length; i++)
            {
                string ln = string.Format("{0} {1}_{2} {3}", "byte", name, i, encoded[i].ToString());
                ParseVariable(ln);
            }

            //address += (ushort)length;
        }

        void ParseArray(string line)
        {
            Match match = Regex.Match(line, arrayCapture);

            string type = match.Groups[1].Value;
            int length = int.Parse(match.Groups[2].Value);
            string name = match.Groups[3].Value;
            string value = match.Groups[4].Value;
            string[] values;
            if (value == "default")
            {
                values = new string[length];
                for (int i = 0; i < values.Length; i++) values[i] = "0";
            }
            else
            {
                values = value.Split(',');
            }

            for (int i = 0; i < length; i++)
            {
                string ln = string.Format("{0} {1}_{2} {3}", type, name, i, values[i]);
                ParseVariable(ln);
            }
        }

        void ParseInstruction(string line)
        {
            Match match = Regex.Match(line, instructionCapture);

            string operation = match.Groups[1].Value;
            string opA = match.Groups[2].Value;
            string opB = match.Groups[4].Value;

            uint16_t operandA = Int.UInt16;
            uint16_t operandB = Int.UInt16;
            OpcodeFlags flags = OpcodeFlags.NoOperands;

            Func<bool> era = () => {
                uint16_t temp;
                if (ExpectRegister(opA, out temp))
                {
                    operandA = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> erb = () => {
                uint16_t temp;
                if (ExpectRegister(opB, out temp))
                {
                    operandB = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> ela = () => {
                uint16_t temp;
                if (ExpectLiteral(opA, out temp))
                {
                    operandA = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> elb = () => {
                uint16_t temp;
                if (ExpectLiteral(opB, out temp))
                {
                    operandB = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> esa = () => {
                uint16_t temp;
                if (flags.HasFlag(OpcodeFlags.Literal1))
                    return false;

                if (ExpectSymbol(opA, out temp))
                {
                    operandA = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            Func<bool> esb = () => {
                uint16_t temp;
                if (flags.HasFlag(OpcodeFlags.Literal2))
                    return false;

                if (ExpectSymbol(opB, out temp))
                {
                    operandB = temp;
                    return true;
                }
                else
                {
                    return false;
                }
            };

            try
            {
                if (era())
                    flags = flags | OpcodeFlags.Register1;
                if (erb())
                    flags = flags | OpcodeFlags.Register2;
                if (ela())
                    flags = flags | OpcodeFlags.Literal1;
                if (elb())
                    flags = flags | OpcodeFlags.Literal2;
                if (esa())
                    flags = flags | OpcodeFlags.Literal1;
                if (esb())
                    flags = flags | OpcodeFlags.Literal2;

                Opcodes opcode = ParseOpcode(operation, flags);
                Opcode oc = new Opcode(opcode, flags, operandA, operandB);
                buildList.Add(string.Format("${0}", opcodes.Count));
                opcodes.Add(oc);
                address += 8;
            }
            catch
            {
                throw;
            }
        }

        Opcodes ParseOpcode(string operation, OpcodeFlags flags)
        {
            Func<bool> no = () => flags == OpcodeFlags.NoOperands;
            Func<bool> ra = () => flags.HasFlag(OpcodeFlags.Register1);
            Func<bool> rb = () => flags.HasFlag(OpcodeFlags.Register2);
            Func<bool> la = () => flags.HasFlag(OpcodeFlags.Literal1);
            Func<bool> lb = () => flags.HasFlag(OpcodeFlags.Literal2);

            Func<Opcodes, Opcodes, Opcodes, Opcodes, Opcodes> RR_LR_RL_LL = (rr, lr, rl, ll) =>
            {
                if (ra() & rb())
                    return rr;
                else if (ra() & lb())
                    return rl;
                else if (la() & rb())
                    return lr;
                else if (la() & la())
                    return ll;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes, Opcodes> RR_LR = (rr, lr) =>
            {
                if (ra() & rb())
                    return rr;
                if (la() & rb())
                    return lr;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes, Opcodes> RR_RL = (r, l) =>
            {
                if (ra() & rb())
                    return r;
                else if (ra() & lb())
                    return l;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes> NoOps = o =>
            {
                if (no())
                    return o;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes> LR = l =>
            {
                if (lb() | ra())
                    InvalidOperands();
                if (la() & rb())
                    return l;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes, Opcodes> R_L = (r, l) =>
            {
                if (rb() | lb())
                    InvalidOperands();
                if (ra())
                    return r;
                else if (la())
                    return l;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes> R = (r) =>
            {
                if (rb() | lb() | la())
                    InvalidOperands();
                if (ra())
                    return r;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            Func<Opcodes, Opcodes> L = (l) =>
            {
                if (rb() | lb() | ra())
                    InvalidOperands();
                if (la())
                    return l;
                else
                {
                    InvalidOperands();
                    return Opcodes.nop;
                }
            };

            switch (operation)
            {
                case "nop":
                    return NoOps(Opcodes.nop);

                case "read":
                    return RR_RL(Opcodes.readr, Opcodes.readl);

                case "write":
                    return RR_LR_RL_LL(Opcodes.writerr, Opcodes.writelr, Opcodes.writerl, Opcodes.writell);

                case "push":
                    return R_L(Opcodes.pushr, Opcodes.pushl);

                case "mov":
                    return RR_RL(Opcodes.movr, Opcodes.movl);

                case "add":
                    return RR_RL(Opcodes.addr, Opcodes.addl);

                case "sub":
                    return RR_RL(Opcodes.subr, Opcodes.subl);

                case "mult":
                    return RR_RL(Opcodes.multr, Opcodes.multl);

                case "div":
                    return RR_RL(Opcodes.divr, Opcodes.divl);

                case "mod":
                    return RR_RL(Opcodes.modr, Opcodes.modl);

                case "and":
                    return RR_RL(Opcodes.andr, Opcodes.andl);

                case "or":
                    return RR_RL(Opcodes.orr, Opcodes.orl);

                case "xor":
                    return RR_RL(Opcodes.xorr, Opcodes.xorl);

                case "not":
                    return R(Opcodes.not);

                case "neg":
                    return R(Opcodes.neg);

                case "pop":
                    return R(Opcodes.pop);

                case "pusha":
                    return NoOps(Opcodes.pusha);

                case "popa":
                    return NoOps(Opcodes.popa);

                case "hlt":
                    return NoOps(Opcodes.hlt);

                case "int":
                    return R_L(Opcodes.intr, Opcodes.intl);

                case "call":
                    return R_L(Opcodes.callr, Opcodes.calll);

                case "jmp":
                    return R_L(Opcodes.jmpr, Opcodes.jmpl);

                case "je":
                    return L(Opcodes.je);

                case "jne":
                    return L(Opcodes.jne);

                case "jg":
                    return L(Opcodes.jg);

                case "jge":
                    return L(Opcodes.jge);

                case "jl":
                    return L(Opcodes.jl);

                case "jle":
                    return L(Opcodes.jle);

                case "cmp":
                    return RR_RL(Opcodes.cmpr, Opcodes.cmpl);

                case "ret":
                    return NoOps(Opcodes.ret);

                case "deref":
                    return RR_RL(Opcodes.derefr, Opcodes.derefl);

                case "brk":
                    return NoOps(Opcodes.brk);

                case "cpuid":
                    return NoOps(Opcodes.cpuid);

                case "out":
                    return NoOps(Opcodes._out);

                case "in":
                    return NoOps(Opcodes._in);

                case "sa":
                    return RR_LR_RL_LL(Opcodes.sarr, Opcodes.salr, Opcodes.sarl, Opcodes.sall);

                case "sai":
                    return R_L(Opcodes.sair, Opcodes.sail);

                case "inca":
                    return NoOps(Opcodes.inca);

                case "deca":
                    return NoOps(Opcodes.deca);

                case "ls":
                    return RR_RL(Opcodes.lsr, Opcodes.lsl);

                case "rs":
                    return RR_RL(Opcodes.rsr, Opcodes.rsl);
            }

            RaiseError(string.Format("Invalid instruction \"{0}\"", operation));
            return Opcodes.nop;
        }

        /// <summary>
        /// Resolves unresolved symbols
        /// </summary>
        //void ResolveSymbols()
        //{
        //    string[] urs = unresolvedSymbols.Keys.ToArray();

        //    for (int i = 0; i < urs.Length; i++)
        //    {
        //        string symbol = urs[i];
        //        int index = unresolvedSymbols[symbol];
        //        Opcode o = opcodes[index];

        //        if (symbols.ContainsKey(symbol))
        //        {
        //            IDataType value = symbols[symbol];

        //            if (o.Flags.HasFlag(OpcodeFlags.Literal1))
        //            {
        //                if (value.Address > ushort.MaxValue)
        //                {
        //                    RaiseError("32-bit address support not available.");
        //                }
        //                else
        //                {
        //                    o.OperandA.Value = (ushort)value.Address;
        //                }
        //            }
        //            else if (o.Flags.HasFlag(OpcodeFlags.Literal2))
        //            {
        //                if (value.Address > ushort.MaxValue)
        //                {
        //                    RaiseError("32-bit address support not available.");
        //                }
        //                else
        //                {
        //                    o.OperandB.Value = (ushort)value.Address;
        //                }
        //            }
        //            else
        //            {
        //                RaiseError(string.Format("Symbol \"{0}\" could not be resolved.", symbol));
        //            }
        //        }
        //        else
        //        {
        //            RaiseError(string.Format("Symbol \"{0}\" not found and could not be resolved.", symbol));
        //        }
        //    }
        //}

        void ResolveSymbols()
        {
            for (int i = 0; i < unresolvedSymbols.Count; i++)
            {
                string symbol = unresolvedSymbols[i].symbol;
                int index = unresolvedSymbols[i].location;
                Opcode o = opcodes[index];

                if (symbols.ContainsKey(symbol))
                {
                    IDataType value = symbols[symbol];

                    if (o.Flags.HasFlag(OpcodeFlags.Literal1))
                    {
                        if (value.Address > ushort.MaxValue)
                        {
                            RaiseError("32-bit address support not available.");
                        }
                        else
                        {
                            o.OperandA.Value = (ushort)value.Address;
                        }
                    }
                    else if (o.Flags.HasFlag(OpcodeFlags.Literal2))
                    {
                        if (value.Address > ushort.MaxValue)
                        {
                            RaiseError("32-bit address support not available.");
                        }
                        else
                        {
                            o.OperandB.Value = (ushort)value.Address;
                        }
                    }
                    else
                    {
                        RaiseError(string.Format("Symbol \"{0}\" could not be resolved.", symbol));
                    }
                }
                else
                {
                    RaiseError(string.Format("Symbol \"{0}\" not found and could not be resolved.", symbol));
                }
            }
        }

        void ResolvePragmas()
        {
            if (pragma.ContainsKey("len") && symbols.ContainsKey(pragma["len"]))
            {
                if (stage == Stage.Preassembly)
                {
                    ((uint32_t)(symbols[pragma["len"]])).Value = Length;
                }
            }
            if (pragma.ContainsKey("offset"))
            {
                if (stage == Stage.Preparsing)
                {
                    ushort offset = ushort.Parse(pragma["offset"]);
                    Offset = offset;
                }
            }
            if (pragma.ContainsKey("encoding"))
            {
                if (stage == Stage.Preparsing)
                {
                    switch (pragma["encoding"])
                    {
                        case "437":
                            encoding = Encoding.GetEncoding(437);
                            break;

                        case "ascii":
                            encoding = Encoding.ASCII;
                            break;

                        case "utf8":
                            encoding = Encoding.UTF8;
                            break;

                        default:
                            encoding = Encoding.ASCII;
                            break;
                    }
                }
            }
            if (pragma.ContainsKey("pad"))
            {
                int len;

                if (stage == Stage.Postassembly)
                {
                    if (int.TryParse(pragma["pad"], out len))
                    {
                        if (binary.Count < len)
                        {
                            byte[] padding = new byte[len - binary.Count];
                            binary.AddRange(padding);
                        }
                    }
                }
            }
        }
    }
}
