using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /* Represents a terminal (return) type within the AST,
     * such as a number, boolean, String etc
     */
    public class TerminalNode : Node
    {
        public TerminalNode(Token token) : base(token) { }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
