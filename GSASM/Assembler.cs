using System;
using System.Collections.Generic;
using System.Text;
using GSVM.Compiler;
using GSVM.Assembler.Targets;

namespace GSVM.Assembler
{
    /// <summary>
    /// Represents the assembler parser. 
    /// </summary>
    public class Assembler
    {
        Tokenizer tokenizer = new Tokenizer();
        string[] tokens;

        internal List<byte> assembly = new List<byte>();

        public byte[] Assembly { get { return assembly.ToArray(); } }

        public string[] Tokens { get { return tokens; } }

        Dictionary<string, Target> targets;
        Target target;

        public Encoding encoding = Encoding.GetEncoding(437);

        List<string> errors = new List<string>();

        public Assembler(string target)
        {
            tokenizer.AddDelimiters(' ');
            tokenizer.AddOperators("\r\n", "\n\r", "\r", "\n");
            tokenizer.AddOperators(",", "+", "-", "[", "]", "{", "}", "@");
            tokenizer.StatementTerminator = '\0';
            tokenizer.SingleLineComment = ";";
            tokenizer.MultiLineCommentStart = tokenizer.MultiLineCommentEnd = "";
            tokenizer.StringStart = '\"';
            tokenizer.StringEnd = '\"';
            tokenizer.CharStart = '\'';
            tokenizer.CharEnd = '\'';

            targets = new Dictionary<string, Target>();
            targets.Add("CPU1", new CPU1(this));

            this.target = targets[target];
        }

        public byte[] Assemble(string code)
        {
            tokens = tokenizer.Tokenize(code);

            for (int i = 0; i < tokens.Length; i++)
            {
                target.Assemble(i);
            }

            return Assembly;
        }

        internal void Error(string s)
        {
            errors.Add(s);
        }

        internal void Error(string format, params object[] args)
        {
            errors.Add(string.Format(format, args));
        }
    }
}
