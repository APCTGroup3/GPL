using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CoreParser
{

    public class Lexer
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
            if (code.Length > 2 && (code.Substring(0, 2) == "lt" || code.Substring(0, 2) == "gt" || code.Substring(0, 2) == "<=" || code.Substring(0, 2) == ">=" || code.Substring(0, 2) == "==" || code.Substring(0, 2) == "!="))
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 2),
                    tokenType = TokenTypes.op,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(2);
                return true;
            }

            if (code[0] == '=' || code[0] == '>' || code[0] == '<' || code[0] == '+' || code[0] == '-' || code[0] == '/' || code[0] == '*' || (code[0] == '.' && tokenList[tokenList.Count - 1].tokenType != TokenTypes.constant) || code[0] == '^' || code[0] == '(' || code[0] == ')')
            {
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, 1),
                    tokenType = TokenTypes.op,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(1);
                return true;
            }



            return false;
        }

        private bool isConstant(ref string code, int lineNum)
        {
            if (code[0] == '.' && tokenList[tokenList.Count - 1].tokenType == TokenTypes.constant)
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

            if (code[0] == '"' | code[0] == '\'' ) //string literal
            {
                char delimiter = code[0];
                string str = "";
                int i = 1;
                while (code[i] != delimiter)
                {
                    str += code[i];
                    i++;
                }
                var tok = new Token()
                {
                    token = code.Substring(1, i - 1),
                    tokenType = TokenTypes.constant,
                    constType = ConstTypes.str,
                    lineNumber = lineNum
                };

                tokenList.Add(tok);
                code = code.Substring(i + 1);
                return true;
            }

            if (isDigit(code[0]))
            {
                String num = string.Empty;
                int i = 0;
                while (i < code.Length && ( isDigit(code[i]) || code[i] == '.') )
                {
                    i++;
                }

                // is a digit
                // Create a new token with the correct type and the part of the source code it is scanning
                Token tok = new Token()
                {
                    token = code.Substring(0, i),
                    tokenType = TokenTypes.constant,
                    constType = ConstTypes.number,
                    lineNumber = lineNum
                };

                this.tokenList.Add(tok);
                code = code.Substring(i);
                return true;
            }

            if (Char.IsLetter(code[0]))
            {
                //Else assume variable or keyword for now, delimit by space
                String chars = "";
                int length = 0;
                bool stop = false;
                while (length < code.Length && !stop)
                {
                    if (!Char.IsWhiteSpace(code[length]))
                    {
                        chars += code[length];
                        length++;
                    }
                    else
                    {
                        stop = true;
                    }
                }

                //Is boolean?
                Token tok;
                if (new String[]{"TRUE"}.Contains(chars.ToUpper()))
                {
                    tok = new Token()
                    {
                        token = "true",
                        tokenType = TokenTypes.constant,
                        constType = ConstTypes.boolean,
                        lineNumber = lineNum
                    };
                } 
                else if (new String[] { "FALSE" }.Contains(chars.ToUpper()))
                {
                    tok = new Token()
                    {
                        token = "false",
                        tokenType = TokenTypes.constant,
                        constType = ConstTypes.boolean,
                        lineNumber = lineNum
                    };
                }
                else
                {
                    tok = new Token()
                    {
                        token = chars,
                        tokenType = TokenTypes.identity,
                        lineNumber = lineNum
                    };
                }

                this.tokenList.Add(tok);
                code = code.Substring(length);


                return true;
            }

            return false;
        }

        private bool isDigit(char code)
        {
            if (code >= '0' && code <= '9')
            {
                return true;
            }
            else
            {
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
                    tokenType = TokenTypes.constant,
                    constType = ConstTypes.boolean,
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
                    tokenType = TokenTypes.statement,
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
                    tokenType = TokenTypes.statement,
                };

                this.tokenList.Add(tok);

                //Remove the token from the source code
                code = code.Substring(2);

                return true;
            }

            return false;
        }


        /* PUBLIC */

        public Lexer(string sourceCode)
        {
            this.sourceCode = sourceCode;
        }


        public void Tokenise()
        {
            string code = this.sourceCode; //Probably should just pass code as param

            int charNum = 0;
            int lineNum = 0;

            while (code != string.Empty)
            {

                Debug.WriteLine(code);

                // Check if the character is an operator
                if (this.isOperator(ref code, lineNum))
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
                if (code.Length > 2 && code.Substring(0, 2) == "\n")
                {
                    lineNum++;
                    charNum = 0;
                }
                else
                {
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
