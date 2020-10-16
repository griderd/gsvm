using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace GSVM.Components.Processors.CPU_1.Assembler
{
    public class Preprocessor
    {
        List<string> code;
        Dictionary<string, string> definitions;
        Dictionary<string, string> pragmas;

        public Dictionary<string, string> Pragmas { get { return pragmas; } }

        public Preprocessor()
        {
            code = new List<string>();
            definitions = new Dictionary<string, string>();
            pragmas = new Dictionary<string, string>();
        }

        public void AddCode(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                try
                {
                    if (line.StartsWith("%include "))
                    {
                        Include(line);
                    }
                    else
                    {
                        code.Add(line);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public string[] ProcessCode()
        {
            List<string> phase1 = new List<string>();
            List<string> result = new List<string>();

            for (int i = 0; i < code.Count; i++)
            {
                string line = code[i];

                if (line.StartsWith("%define "))
                {
                    Define(line);
                }
                else if (line.StartsWith("%undef "))
                {
                    Undefine(line);
                }
                else if (line.StartsWith("%pragma "))
                {
                    Pragma(line);
                }
                else if (line.StartsWith("#") | line.StartsWith(";") | (line == ""))
                {
                    // DO NOTHING
                }
                else if (line.EndsWith(":"))
                {
                    phase1.Add("");
                    phase1.Add(line);
                }
                else
                {
                    phase1.Add(line);
                }
            }

            for (int i = 0; i < phase1.Count; i++)
            {
                string line = phase1[i];

                line = Substitute(line);
                result.Add(line);
            }

            return result.ToArray();
        }

        void Include(string line)
        {
            int inc = "%include ".Length;
            string path = line.Substring(inc, line.Length - inc);
            if (definitions.ContainsKey(path))
                path = definitions[path];

            try
            {
                string[] lines = File.ReadAllLines(path);
                AddCode(lines);
            }
            catch
            {
                throw;
            }
        }

        void Define(string line)
        {
            Match match = Regex.Match(line, "%define (.*?) (.*)");
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (definitions.ContainsKey(key))
                    definitions[key] = value;
                else
                    definitions.Add(key, value);
            }
            else
            {
                match = Regex.Match(line, "%define (.*)");
                if (!match.Success)
                    return;
                string key = match.Groups[1].Value;
                if (!definitions.ContainsKey(key))
                    definitions.Add(key, "");
            }
        }

        void Undefine(string line)
        {
            int inc = "%undef ".Length;
            string key = line.Substring(inc, line.Length - inc);

            if (definitions.ContainsKey(key))
                definitions.Remove(key);
        }

        void Pragma(string line)
        {
            Match match = Regex.Match(line, "%pragma (.*?) (.*)");
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (pragmas.ContainsKey(key))
                    pragmas[key] = value;
                else
                    pragmas.Add(key, value);
            }
        }

        string Substitute(string line)
        {
            string result = line;

            string[] keys = definitions.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (result.Contains(keys[i]))
                    result = result.Replace(keys[i], definitions[keys[i]]);
            }

            return result;
        }
    }
}
