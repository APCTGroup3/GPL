using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    public class FunctionNode : Node
    {
        public List<Node> Parameters { get; set; }
        public FunctionNode(Token token) : this(token, new Node[0]) {}
        public FunctionNode(Token token, params Node[] parameters) : base(token)
        {
            Parameters = new List<Node>();
            foreach (Node node in parameters)
            {
                Parameters.Add(node);
            }
        }

        public override List<Node> Children()
        {
            return Parameters;
        }
    }
}
