using System;
namespace CoreParser.Parser.AST
{
    public abstract class Expression : Node
    {
        protected Expression(Token token) : base(token) { }
    }
}
