using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Interpreter{

    class Lexer
    {
        /* Interesting Links

        * https://hackernoon.com/lexical-analysis-861b8bfe4cb0
        * https://jack-vanlightly.com/blog/2016/2/3/creating-a-simple-tokenizer-lexer-in-c
        * 
        ------------------------------ 
        */

        private List<Token> tokenList = new List<Token>();
        private string sourceCode = string.Empty;


        public Lexer(string sourceCode){
            this.sourceCode = sourceCode;
        }


        public void Tokenise(){
            string code = this.sourceCode; //Probably should just pass code as param
            int charNum = 0;
            int lineNum = 0;
            while(code.Length > 0){

                Console.WriteLine(code);

                // Check if the character is an operator
                if(isOperator(ref code, lineNum))
                {
                    Console.WriteLine("Operator found on line " + lineNum);
                    continue;
                }

                // Check if the character is an statement
                if (isStatement(ref code, lineNum))
                {
                    Console.WriteLine("Statement found on line " + lineNum);
                    continue;
                }

                charNum++;

                if(code.Substring(0, 2) == "\n"){
                    lineNum++;
                    charNum = 0;
                }
                code = code.Substring(1);
            }
        }

        public List<Token> getTokenList()
        {
            return tokenList;
        }

        private bool isOperator( ref string code, int lineNum)
        {
            if(code[0] == '>' || code[0] == '<' || code[0] == '+' || code[0] == '-' || code[0] == '/' || code[0] == '*')
            {
                Token tok = new Token()
                {
                    token = code.Substring(0,1),
                    type = TokenTypes.expression,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                code = code.Substring(1);
                return true;
            }
            return false;
        }

        private bool isStatement(ref string code, int lineNum)
        {
            if (code.Length > 4 && (code.Substring(0, 4) == "else" || code.Substring(0, 4) == "then"))
            {
                Token tok = new Token()
                {
                    token = code.Substring(0, 4),
                    type = TokenTypes.statement,
                    lineNumber = lineNum
                };
                this.tokenList.Add(tok);

                code = code.Substring(5);
                return true;
            }
            if (code.Length >= 2 && code.Substring(0,2) == "if")
            {
                Token tok = new Token()
                {
                    token = code.Substring(0, 2),
                    type = TokenTypes.statement,
                };
                this.tokenList.Add(tok);

                code = code.Substring(2);
                return true;
            }

            return false;
        }
    }
}
