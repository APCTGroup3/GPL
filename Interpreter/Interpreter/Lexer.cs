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

        /* 
         * The regex is me trying a different way of tokenising, I was trying to see if we could not rely on whitespace but it's much easier using whitespace as delim due to syntax complexity
         * Could probably use regex to categorise tokens or possibly implement a "smarter" lexer later on, for now though this should work pretty well
         */

        //private Regex regex = new Regex(@"AND|&|and|&&|OR|or|\|\||PLUS|\+|plus|add|ADD|\-|minus|sub|SUB|then|THEN|>|greater than|gt|greaterthan");


        public Lexer(string sourceCode){
            this.sourceCode = sourceCode;
        }


        public void Tokenise(){
            string code = this.sourceCode; //Probably should just pass code as param
            while(code.Length > 0){

                Console.WriteLine(code);

                // Check if the character is an operator
                if(isOperator(ref code))
                {
                    Console.WriteLine("Operator found");
                    continue;
                }

                // Check if the character is an statement
                if (isStatement(ref code))
                {
                    Console.WriteLine("Statement found");
                    continue;
                }


                code = code.Substring(1);
            }
        }

        public List<Token> getTokenList()
        {
            return tokenList;
        }

        private bool isOperator( ref string code)
        {
            if(code[0] == '>' || code[0] == '<' || code[0] == '+' || code[0] == '-' || code[0] == '/' || code[0] == '*')
            {
                code = code.Substring(1);
                return true;
            }
            return false;
        }

        private bool isStatement(ref string code)
        {
            if (code.Length >= 2 && code.Substring(0,2) == "if")
            {
                code = code.Substring(2);
                return true;
            }
            if (code.Length >= 5 && code.Substring(0, 5) == "else")
            {
                code = code.Substring(5);
                return true;
            }

            return false;
        }
    }
}




/*
string[] stringToks = Regex.Split(this.sourceCode, " |$"); //Split at whitespace or newline

            foreach(string tok in stringToks){ // Works for now but should probably be replaced by a classic for loop e.g for(int i = 0; ... Because that will allow us to store character number info
                tokenList.Add(new Token()
                {
                    type = this.findType(tok),
                    token = tok
                });
            }

            /* FOR DEBUGGING */
/*
Console.WriteLine(tokenList.Count); // Look at how many tokens were found
*/


/*
             * Categorises an input string based on what that string contains
             * Should include a way of categorising an incorrectly formatted string i.e a+b and return a helpful error message e.g "a+b is invalid, please seperate with whitespace"
             */
/*
            switch(tok){
                case "+":
                return TokenTypes.expression;

                case "-":
                return TokenTypes.expression;

                case "/":
                return TokenTypes.expression;

                case "*":
                return TokenTypes.expression;

                case "%":
                return TokenTypes.expression;
                
                case "^":
                return TokenTypes.expression;
                
                default:
                return TokenTypes.identity;
            }
            */