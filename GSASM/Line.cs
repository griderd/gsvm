using System;
using System.Collections.Generic;
using System.Text;

namespace GSVM.Assembler
{
    struct Line
    {
        public string File { get; private set; }
        public int LineNumber { get; private set; }
        public string Text { get; private set; }

        string[] tokens;
        public string[] Tokens { get; private set; }

        public Line(string file, int index, string line, string[] tokens) : this()
        {
            File = file;
            LineNumber = index + 1;
            Text = line;
            Tokens = tokens;
        }
    }
}
