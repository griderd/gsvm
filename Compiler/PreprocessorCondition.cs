using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Compiler
{
    struct PreprocessorCondition
    {
        ShuntingYard shuntingYard;

        public ConditionalState state;
        public string condition;

        public PreprocessorCondition(ConditionalState _state, string _condition)
        {
            state = _state;
            condition = _condition;
            shuntingYard = new ShuntingYard(new string[] { "+", "-", "*", "/", "%", "!", "=", "==", "<", ">", "<=", ">=", "defined" });
        }

        public bool Evaluate(Preprocessor p)
        {
            //switch (state)
            //{
            //    case ConditionalState.IfDef:

            //}

            return false;
        }
    }
}
