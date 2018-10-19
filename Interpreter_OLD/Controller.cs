using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Interpreter{
    class Controller{
        static void Main(string[] args){
            Lexer lexer = new Lexer("if x > y then a-b");
            lexer.tokenise();
        }
    }
}