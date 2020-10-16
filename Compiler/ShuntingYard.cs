using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Compiler
{
    public class ShuntingYard
    {
        Tokenizer tokenizer = new Tokenizer();
        Queue<string> output = new Queue<string>();
        Stack<string> operators = new Stack<string>();

        public Func<string, bool> IsFunction;
        public Func<string, bool> IsVariable;

        string[] definedOperators;

        public ShuntingYard(string[] operators)
        {
            tokenizer.AddDelimiters(' ');
            tokenizer.AddOperators(operators);
            tokenizer.AddOperators("(", ")");
            definedOperators = operators;
        }

        public string GenerateRPN(string postfix)
        {
            string[] tokens = tokenizer.Tokenize(postfix);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                double temp;

                // If token is number
                if (double.TryParse(token, out temp))
                    output.Enqueue(token);
                if (IsVariable?.Invoke(token) == true)
                    output.Enqueue(token);
                if (IsFunction?.Invoke(token) == true)
                    operators.Push(token);

                // If token is operator
                if (definedOperators.Contains(token))
                {
                    // While there is a function on top of the stack
                    while (!definedOperators.Contains(operators.Peek()))
                    {

                    }
                }
            }

            return "";
        }
    }
}
