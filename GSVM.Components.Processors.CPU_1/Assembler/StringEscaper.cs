using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSVM.Components.Processors.CPU_1.Assembler
{
    public class StringEscaper
    {
        static IDictionary<string, string> replace = new Dictionary<string, string>();
        static readonly string[] keys = { @"\a", @"\b", @"\f", @"\n", @"\t", @"\v", @"\\", @"\0" };
        static readonly char[] values = { '\a', '\b', '\f', '\n', '\t', '\v', '\\', '\0' };

        public static string StringLiteral(string s)
        {
            string str = s;
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                string value = values[i].ToString();

                str = str.Replace(key, value);
            }
            return str;
        }

        public static char CharLiteral(string c)
        {
            if (c.StartsWith("\\") & (c.Length == 2))
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    char value = values[i];

                    if (c == key)
                        return value;
                }
            }
            else if (c.Length == 1)
                return c[0];
            else
                throw new ArgumentException("Not a valid character.");

            return '\0';
        }

       
    }
}
