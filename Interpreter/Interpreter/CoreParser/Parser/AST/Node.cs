 using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public abstract class Node
    {
        public Token Token { get; protected set; }

        protected Node(Token token)
        {
            Token = token;
        }

        public abstract List<Node> Children();
        public bool HasChildren()
        {
            return Children().Count > 0;
        }
    }
}
