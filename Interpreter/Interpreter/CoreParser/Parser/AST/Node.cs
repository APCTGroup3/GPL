 using System;
using System.Collections.Generic;

namespace CoreParser.Parser.AST
{
    /** Represents a single node within an Abstract Syntax Tree **/
    public abstract class Node
    {
        //The lexer token corresponding to the given node
        public Token Token { get; protected set; }

        /*
         * Creates a simple blank Node consisting of a single Token       
         */        
        protected Node(Token token)
        {
            Token = token;
        }

        /*
         * Returns a list of all immediate children of this Node.
         * Implementations should ensure that the children are returned in a logic order,
         * usually the order they would appear within the source code
         */
        public abstract List<Node> Children();

        /*
         * Returns true if the current Node is internal, and false if it is a leaf node.
         */
        public bool HasChildren()
        {
            return Children().Count > 0;
        }
    }
}
