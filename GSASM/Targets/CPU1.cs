using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GSVM.Compiler;

namespace GSVM.Assembler.Targets
{
    public class CPU1 : Target
    {
        int id;

        public static string Name { get { return "CPU1"; } }

        string Token { get { return asm.Tokens[id]; } }

        Dictionary<string, ushort> symbols = new Dictionary<string, ushort>();
        List<UnresolvedSymbol> unresolvedSymbols = new List<UnresolvedSymbol>();

        List<string> types = new List<string>(new string[]
        {
            "uint8", "byte", "int8", "sbyte",
            "ushort", "uint16", "short", "int16", "ptr",
            "uint", "uint32", "int", "int32", "lptr",
            "ulong", "uint64", "long", "int64",
            "float", "double", "string"
        });

        #region Instructions

        List<string> instructions = new List<string>();

        List<string> noOps = new List<string>(new string[]
        {
            "nop",
            "popa",
            "hlt",
            "brk",
            "cpuid",
            "out",
            "in"
        });

        List<string> r = new List<string>(new string[]
        {
            "not",
            "neg",
            "pop"
        });

        List<string> l = new List<string>(new string[]
        {
            "je",
            "jne",
            "jg",
            "jge",
            "jl",
            "jle"
        });

        List<string> r_l = new List<string>(new string[]
        {
            "push",
            "int",
            "call",
            "jmp"
        });

        List<string> rr_lr = new List<string>(new string[]
        {
            "write"
        });

        List<string> rr_rl = new List<string>(new string[]
        {
            "read",
            "mov",
            "add",
            "sub",
            "mult",
            "div",
            "mod",
            "and",
            "or",
            "xor",
            "cmp",
            "deref"
        });

        #endregion 

        public CPU1(Assembler asm)
            : base(asm)
        {
            instructions.AddRange(noOps);
            instructions.AddRange(r);
            instructions.AddRange(l);
            instructions.AddRange(r_l);
            instructions.AddRange(rr_lr);
            instructions.AddRange(rr_rl);
        }

        public override int Assemble(int tokenID)
        {
            id = tokenID;

            if (types.Contains(Token))
                ParseVariable();
            else if (instructions.Contains(Token))
                ParseInstruction();

            return id;
        }

        #region General Helpers

        /// <summary>
        /// Determines if the current token is an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ExpectInteger(out int value)
        {
            return int.TryParse(Token, out value);
        }

        bool ExpectChar(out char value)
        {
            value = '\0';
            if (Token != "\'")
            {
                asm.Error("Expected character.");
                return false;
            }

            id++;
            try
            {
                value = StringEscaper.CharLiteral(Token);
            }
            catch (ArgumentException)
            {
                asm.Error("Invalid character.");
                return false;
            }

            id++;
            if (Token != "\'")
            {
                asm.Error("Expected character.");
                return false;
            }

            return true;
        }

        #endregion

        #region Variable

        void ParseVariable()
        {
            string type = Token;
            int size = SizeOf(Token);
            int length = 1;
            byte[] value;

            bool error = false;

            if (ExpectArrayLength(out length, out error))
            {
                value = new byte[size * length];
            }
            else
            {
                length = 1;
                value = new byte[size];
            }

            if (error)
            {
                asm.Error("Invalid array length.");
                return;
            }

            if (!ExpectName())
            {
                asm.Error("Invalid variable name.");
                return;
            }

            if (length == 1)
                value = ParseValue(type);
            else
                value = ParseArrayValue(type, length);
        }

        bool ExpectName()
        {
            id++;
            if (Regex.IsMatch(Token, "[a-zA-Z_]{1}[a-zA-Z0-9_\\[\\]]*"))
            {
                symbols.Add(Token, (ushort)asm.assembly.Count);
                return true;
            }

            return false;
        }

        bool IsSymbol()
        {
            return Regex.IsMatch(Token, "[a-zA-Z_]{1}[a-zA-Z0-9_\\[\\]]*");
        }

        bool ExpectArrayLength(out int length, out bool error)
        {
            length = 1;
            error = false;

            id++;
            if (Token == "[")
            {
                id++;
                if (!ExpectInteger(out length))
                {
                    error = true;
                    return false;
                }

                id++;
                if (Token == "]")
                    return true;
                else
                    error = true;
            }

            return false;
        }

        byte[] ParseValue(string type)
        {
            switch (type)
            {
                case "uint8":
                case "byte":
                    byte uint8 = 0;
                    if (!byte.TryParse(Token, out uint8))
                        asm.Error("Invalid value for type uint8.");
                    return new byte[] { uint8 };

                case "int8":
                case "sbyte":
                    sbyte int8 = 0;
                    if (!sbyte.TryParse(Token, out int8))
                        asm.Error("Invalid value for type int8.");
                    return new byte[] { (byte)int8 };

                case "char":
                    char ch = '\0';
                    ExpectChar(out ch);
                    return asm.encoding.GetBytes(new char[] { ch });

                case "uint16":
                case "ushort":
                    ushort uint16 = 0;
                    if (!ushort.TryParse(Token, out uint16))
                        asm.Error("Invalid value for type uint16.");
                    return BitConverter.GetBytes(uint16);

                case "int16":
                case "short":
                    short int16 = 0;
                    if (!short.TryParse(Token, out int16))
                        asm.Error("Invalid value for type int16.");
                    return BitConverter.GetBytes(int16);

                case "ptr":
                    ushort ptr = ParsePointer();
                    return BitConverter.GetBytes(ptr);

                case "uint32":
                case "uint":
                    uint uint32 = 0;
                    if (!uint.TryParse(Token, out uint32))
                        asm.Error("Invalid value for type uint32.");
                    return BitConverter.GetBytes(uint32);

                case "int32":
                case "int":
                    int int32 = 0;
                    if (!int.TryParse(Token, out int32))
                        asm.Error("Invalid value for type int32.");
                    return BitConverter.GetBytes(int32);

                case "float":
                    float f = 0;
                    if (!float.TryParse(Token, out f))
                        asm.Error("Invalid value for type float.");
                    return BitConverter.GetBytes(f);

                case "uint64":
                case "ulong":
                    ulong uint64 = 0;
                    if (!ulong.TryParse(Token, out uint64))
                        asm.Error("Invalid value for type uint64.");
                    return BitConverter.GetBytes(uint64);

                case "int64":
                case "long":
                    long int64 = 0;
                    if (!long.TryParse(Token, out int64))
                        asm.Error("Invalid value for type int64.");
                    return BitConverter.GetBytes(int64);

                case "double":
                    double d = 0;
                    if (!double.TryParse(Token, out d))
                        asm.Error("Invalid value for type double.");
                    return BitConverter.GetBytes(d);

                case "string":
                    string s = "";
                    ExpectString(out s);
                    return asm.encoding.GetBytes(s);

                default:
                    return new byte[0];
            }
        }

        byte[] ParseArrayValue(string type, int len)
        {
            List<byte> value = new List<byte>();

            id++;
            if (Token != "{")
            {
                switch (Token)
                {
                    case "default":
                        return new byte[SizeOf(type) * len];

                    default:
                        asm.Error("Expecting \"{\" or \"default\"");
                        return new byte[0];
                }
                
            }

            for (int i = 0; i < len; i++)
            {
                id++;
                value.AddRange(ParseValue(type));

                id++;
                if ((i + 1 < len) & (Token != ","))
                {
                    asm.Error("Expected \",\"");
                    return new byte[0];
                }
            }

            if (Token != "}")
            {
                asm.Error("Expecting \"}\"");
                return new byte[0];
            }

            return value.ToArray();
        }

        ushort ParsePointer()
        {
            id++;
            if (Token == "@")
            {
                // The next token should be a symbol
                id++;
                if (IsSymbol())
                {
                    // Check if the symbol is in the symbols list
                    string symbol = Token;
                    if (symbols.ContainsKey(symbol))
                    {
                        return symbols[symbol];
                    }
                    else
                    {
                        unresolvedSymbols.Add(new UnresolvedSymbol() { symbol = Token, location = asm.assembly.Count });
                        return 0;
                    }
                }
                else
                {
                    asm.Error("Invalid symbol.");
                    return 0;
                }
            }
            else
            {
                id++;
                ushort uint16 = 0;
                if (!ushort.TryParse(Token, out uint16))
                    asm.Error("Invalid value for type uint16.");
                return uint16;
            }
        }

        int SizeOf(string type)
        {
            switch (type)
            {
                case "uint8":
                case "int8":
                case "byte":
                case "sbyte":
                case "char":
                    return 1;

                case "uint16":
                case "int16":
                case "short":
                case "ushort":
                case "ptr":
                    return 2;

                case "uint32":
                case "int32":
                case "int":
                case "uint":
                case "lptr":
                case "float":
                    return 4;

                case "uint64":
                case "int64":
                case "long":
                case "ulong":
                case "double":
                    return 8;

                default:
                    return 0;
            }
        }

        #endregion

        #region String

        bool ExpectString(out string value)
        {
            value = "";
            if (Token != "\"")
            {
                asm.Error("Expected string.");
                return false;
            }

            id++;
            try
            {
                value = StringEscaper.CharLiteral(Token).ToString();
            }
            catch (ArgumentException)
            {
                asm.Error("Invalid character.");
                return false;
            }

            id++;
            if (Token != "\'")
            {
                asm.Error("Expected character.");
                return false;
            }

            return true;
        }

        #endregion

        void ParseInstruction()
        {

        }
    }
}
