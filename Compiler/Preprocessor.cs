using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StateMachineLib;

namespace GSVM.Compiler
{
    public class Preprocessor
    {
        Tokenizer tokenizer;
        List<string> lines;
        List<string> result;

        Stack<PreprocessorCondition> conditions;

        internal Dictionary<string, string> macros;

        public Preprocessor()
        {
            lines = new List<string>();
            conditions = new Stack<PreprocessorCondition>();
            macros = new Dictionary<string, string>();

            tokenizer = new Tokenizer();
            tokenizer.CharStart = '<';
            tokenizer.CharEnd = '>';
            tokenizer.StringStart = '\"';
            tokenizer.StringEnd = '\"';
        }

        public string Preprocess(string code)
        {
            // Split code by linebreak
            lines.AddRange(code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (!line.StartsWith("#"))
                {
                    result.Add(lines[i]);
                    continue;
                }

                if (IncludeLibraryFile(i, line)) { i = 0; continue; }
                if (IncludeLocalFile(i, line)) { i = 0; continue; }

            }

            return "";
        }

        bool IncludeLibraryFile(int lineNumber, string line)
        {
            Match m = Regex.Match(line, "#include <(.*)>");

            if (m.Success)
            {
                string filename = m.Captures[1].Value;

                // TODO: Open file and insert it, replacing line number
            }

            return m.Success;
        }

        bool IncludeLocalFile(int lineNumber, string line)
        {
            Match m = Regex.Match(line, "#include \"(.*)\"");

            if (m.Success)
            {
                string filename = m.Captures[1].Value;

                // TODO: Open file and insert it, replacing line number
            }

            return m.Success;
        }
    }
}
