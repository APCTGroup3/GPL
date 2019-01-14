using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class ArrayElement : Node
    {
        public Node Element { get; set; }
        public ArrayElement(Token token, Node element) : base(token) 
        {
            Element = element;
        }

        public override List<Node> Children()
        {
            return new List<Node>();
        }
    }
}
