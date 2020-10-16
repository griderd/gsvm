using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Compiler
{
    public class Tokenizer
    {
        List<string> operators;
        List<char> delimiters;
        List<string> tokens;

        StringBuilder token;
        StringBuilder subtoken;
        char c;
        string code;
        int index;

        bool inString;
        bool inChar;
        bool inSLComment;
        bool inMLComment;

        /// <summary>
        /// Gets or sets the statement terminator. If zero, no statement terminator is used.
        /// </summary>
        public char StatementTerminator { get; set; }

        /// <summary>
        /// Gets or sets the start of a single-line comment. If empty, single-line comments are disabled.
        /// </summary>
        public string SingleLineComment { get; set; }

        /// <summary>
        /// Gets or sets the start of a multi-line comment. If empty, multi-line comments are disabled.
        /// </summary>
        public string MultiLineCommentStart { get; set; }

        /// <summary>
        /// Gets or sets the end of a multi-line comment. If empty, multi-line comments are disabled.
        /// </summary>
        public string MultiLineCommentEnd { get; set; }

        public char StringStart { get; set; }
        public char StringEnd { get; set; }
        public char CharStart { get; set; }
        public char CharEnd { get; set; }

        public Tokenizer()
        {
            operators = new List<string>();
            tokens = new List<string>();
            delimiters = new List<char>();
        }

        public static Tokenizer Freeform(char statementTerminator)
        {
            Tokenizer t = new Tokenizer();
            t.AddDelimiters(' ', '\t', '\r', '\n');
            t.StatementTerminator = statementTerminator;

            return t;
        }

        /// <summary>
        /// Adds token delimiters that define when a token ends. This is typically whitespace.
        /// </summary>
        /// <param name="del"></param>
        public void AddDelimiters(params char[] del)

        {
            delimiters.AddRange(del);
        }

        /// <summary>
        /// Adds operators; tokens that must always be identified even when not next to a delimiter.
        /// </summary>
        /// <param name="keyword"></param>
        public void AddOperators(params string[] keyword)
        {
            operators.AddRange(keyword);
        }

        void CaptureToken()
        {
            if (token.Length != 0)
            {
                tokens.Add(token.ToString());
                StartToken();
            }
        }

        /// <summary>
        /// Tokenizes code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string[] Tokenize(string code)
        {
            this.code = code;
            index = 0;

            StartToken();
            return tokens.ToArray();
        }

        void StartToken()
        {
            token = new StringBuilder();
            subtoken = new StringBuilder();

            while (index < code.Length)
            {
                NextChar();
            }
        }

        void NextChar()
        {
            if (index < code.Length)
            {
                c = code[index];
                index++;
            }
            else
            {
                return;
            }

            if (inSLComment)
            {
                if ((c == '\r') | (c == '\n'))
                {
                    inSLComment = false;
                }
            }
            else if (inMLComment)
            {
                if (Lookahead(MultiLineCommentEnd.Length) == MultiLineCommentEnd)
                {
                    index += MultiLineCommentEnd.Length;
                    inMLComment = false;
                }
            }
            else if (inString)
            {
                token.Append(c);
                if (c == StringEnd)
                {
                    inString = false;
                    CaptureToken();
                }
            }
            else if (inChar)
            {
                token.Append(c);
                if (c == CharEnd)
                {
                    inChar = false;
                    CaptureToken();
                }
            }
            else if (c == StringStart)
            {
                CaptureToken();
                token.Append(c);
                inString = true;
            }
            else if (c == CharStart)
            {
                CaptureToken();
                token.Append(c);
                inChar = true;
            }
            else
            {
                token.Append(c);

                string t = token.ToString();
                if (t == SingleLineComment)
                {
                    inSLComment = true;
                    token.Clear();
                }
                else if (t == MultiLineCommentStart)
                {
                    inMLComment = true;
                }
                else if (operators.Contains(t))
                {
                    bool lookahead = false;

                    for (int i = 0; i < operators.Count; i++)
                    {
                        string kw = operators[i];
                        int len = kw.Length - token.Length;

                        if (len > 0)
                        {
                            string gl = GreedyLookahead(len);

                            if (gl != kw)
                                lookahead = true;
                        }
                    }

                    // If the greedy lookahead failed, there is no longer token with the equivilent beginning
                    // Capture the token instead
                    if (!lookahead)
                        CaptureToken();
                }
            }
        }

        string GreedyLookahead(int count = 1)
        {
            StringBuilder lookahead = new StringBuilder();
            lookahead.Append(token.ToString());
            lookahead.Append(Lookahead(count));

            return lookahead.ToString();
        }

        string Lookahead(int count = 1)
        {
            StringBuilder lookahead = new StringBuilder();

            for (int i = index; ((i < count) & (index + i < code.Length)); i++)
            {
                lookahead.Append(code[index + i]);
            }

            return lookahead.ToString();
        }

        public bool IsOperator(string op)
        {
            return (operators.Contains(op));
        }
    }
}
