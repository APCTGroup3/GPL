using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Interpreter{

    class Lexer
    {
        /* Interesting Links

        * https://hackernoon.com/lexical-analysis-861b8bfe4cb0
        * https://jack-vanlightly.com/blog/2016/2/3/creating-a-simple-tokenizer-lexer-in-c
        * 
        ------------------------------ 
        */


        /* PRIVATE */

        private List<Token> tokenList = new List<Token>();
        private string sourceCode = string.Empty;


        private bool isOperator(ref string code, int lineNum)
        {
            if (code[0] == '>' || code[0] == '<' || code[0] == '+' || code[0] == '-' || code[0] == '/' || code[0] == '*' || (code[0] == '.' && tokenList[tokenList.Count-1].type != TokenTypes.constant) || code[0] == '^' || code[0] == '(' || code[0] == ')')
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 1),
                    type = TokenTypes.op,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(1);
                return true;
            }

            if(code.Length > 2 && (code.Substring(0,2) == "lt" || code.Substring(0, 2) == "gt" || code.Substring(0, 2) == "<=" || code.Substring(0, 2) == ">=") )
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 2),
                    type = TokenTypes.op,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(2);
                return true;
            }

            return false;
        }

        private bool isConstant(ref string code, int lineNum){
            if (code[0] == '.' && tokenList[tokenList.Count - 1].type == TokenTypes.constant)
            {
                Token tok = tokenList[tokenList.Count - 1];
                tok.token += '.';

                int i = 1;
                while (i <= code.Length && isDigit(code[i]))
                {
                    tok.token += code[i];

                    i++;
                }
                this.tokenList.RemoveAt(tokenList.Count - 1);
                this.tokenList.Add(tok);
                code = code.Substring(i);
                return true;
            }

            if (code[0] >= '0' && code[0] <= '9')
            {
                String num = string.Empty;
                int i = 0;
                while(i < code.Length && isDigit(code[i])){
                    i++;
                }

                // is a digit
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, i),
                    type = TokenTypes.constant,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);
                code = code.Substring(i);
                return true;
            }

            return false;
        }

        private bool isDigit(char code){
            if(code >= '0' && code <= '9'){
                return true;
            } else{
                return false;
            }
        }

        private bool isBoolean(ref string code, int lineNum)
        {
            if (code.Length > 4 && (code.Substring(0, 4) == "true" || code.Substring(0, 4) == "TRUE" || code.Substring(0, 4) == "false" || code.Substring(0, 4) == "FALSE"))
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 4),
                    type = TokenTypes.boolean,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(4);

                return true;
            }
            return false;
        }

        private bool isStatement(ref string code, int lineNum)
        {
            if (code.Length > 4 && (code.Substring(0, 4) == "else" || code.Substring(0, 4) == "then"))
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 4),
                    type = TokenTypes.statement,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(4);

                return true;
            }
            if (code.Length >= 2 && code.Substring(0, 2) == "if")
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 2),
                    type = TokenTypes.statement,
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(2);

                return true;
            }

            return false;
        }


        /* PUBLIC */

        public Lexer(string sourceCode){
            this.sourceCode = sourceCode;
        }


        public void Tokenise(){
            string code = this.sourceCode; //Probably should just pass code as param

            int charNum = 0;
            int lineNum = 0;

            while(code != string.Empty){

                Debug.WriteLine(code);

                // Check if the character is an operator
                if(this.isOperator(ref code, lineNum))
                {
                    Debug.WriteLine("Operator found on line " + lineNum);
                    continue;
                }

                // Check if the character is a statement
                if (this.isStatement(ref code, lineNum))
                {
                    Debug.WriteLine("Statement found on line " + lineNum);
                    continue;
                }

                // Check if the character is a constant
                if (this.isConstant(ref code, lineNum))
                {
                    Debug.WriteLine("Constant found on line " + lineNum);
                    continue;
                }

                // Check for new line and increment line number
                if (code.Length > 2 && code.Substring(0, 2) == "\n"){
                    lineNum++;
                    charNum = 0;
                } else {
                    // Don't bother incrementing character number if line number is updating
                    charNum++;
                }

                // Remove first element of source code
                code = code.Substring(1);
            }
        }

        public List<Token> getTokenList()
        {
            return tokenList;
        }
    }
}
