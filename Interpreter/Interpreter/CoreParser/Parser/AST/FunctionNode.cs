using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    //Represents a function call consisting of a function name and a list of parameters
    public class FunctionNode : Node
    {
        public List<Node> Parameters { get; set; } //A list of parameters, in the order they should be passed to the function
        //Token should be the name of the function
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
