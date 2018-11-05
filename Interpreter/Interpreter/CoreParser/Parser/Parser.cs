using System;
using System.Collections.Generic;
using CoreParser.Parser.Ops;

namespace CoreParser.Parser
{
    public class Parser
    {
        private IList<Token> Tokens { get; set; }
        public Node AST { get; private set; }
        private int pos;

        public Node Parse(IList<Token> tokens)
        {

            Tokens = tokens;
            pos = 0;

            AST = ParseExpression();
            return AST;

        }


        private Node ParseComparison()
        {
            var left = ParseAdd();
            if (Match(TokenTypes.op, "<", "lt", "lessthan", "<=", "lte", "lessthanequal", ">", "gt", "greaterthan", ">=", "gte", "greaterthanequal"))
            {
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParseComparison();
                return node;
            }
            else
            {
                return left;
            }
        }

        /****** NUMERIC EXPRESSIONS *******/
        // Any mathematical expression that returns a number
        private Node ParseExpression()
        {
            return ParseAdd();
        }

        private Node ParseAdd()
        {
            var left = ParseMult();
            if (Match(TokenTypes.op, "+", "add", "plus", "-", "subtract"))
            {
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParseAdd();
                return node;
            }
            else
            {
                return left;
            }
        }

        private Node ParseMult()
        {
            var left = ParsePower();
            if (Match(TokenTypes.op, "*", "mult", "multiply", "/", "div", "divide"))
            {
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParseMult();
                return node;
            }
            else
            {
                return left;
            }
        }

        private Node ParsePower()
        {
            var left = ParseParentheses();
            if (CurrentToken.tokenType == TokenTypes.op && CurrentToken.token.Equals("^"))
            {
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParsePower();
                return node;
            }
            else
            {
                return left;
            }
        }

        private Node ParseParentheses()
        {
            if(Match(TokenTypes.op, "("))
            {
                Consume();
                var node = ParseExpression();
                if (Match(TokenTypes.op, ")"))
                {
                    Consume();
                    return node;
                }
                else
                {
                    throw new ParserException("Line " + CurrentToken.lineNumber + ": Expected ), found" + CurrentToken.token);
                }
            }
            else
                return ParseNegative();
        }

        private Node ParseNegative()
        {
            if (Match(TokenTypes.op, "-"))
            {
                var node = new UnaryNode(CurrentToken);
                Consume();
                node.Child = ParseExpression();
                return node;
            }
            else
            {
                return ParseFactor();
            }
        }

        private Node ParseFactor()
        {
            var valid = CurrentToken.tokenType == TokenTypes.constant;
            if (!valid)
            {
                throw new ParserException("Line " + CurrentToken.lineNumber + ": Expected factor; found " + CurrentToken.token);
            } else
            {
                var node = new TerminalNode(CurrentToken);
                Consume();
                return node;

            }
        }

        //public static void Main(string[] args)
        //{
        //    var parser = new Parser();

        //    Lexer l = new Lexer("(453.6^4/3)+64-2.2^5");
        //    l.Tokenise();
        //    foreach (var token in l.getTokenList())
        //    {
        //        Console.Write("\"" + token.token + "\", " );
        //    }
        //    Console.Write("\n");

        //    var tokens = new List<Token>(); //(47.6^(4)*49)+59*43/(4-3)^2
        //    tokens.Add(new Token() { token = "(" , type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "47.6", type=TokenTypes.constant });
        //    tokens.Add(new Token() { token = "^", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "(" , type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "4", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = ")" , type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "*", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "49", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = "+", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "59", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = "*", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "-", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "43", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = "/", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "(", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "4", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = "-", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "3", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = ")", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "^", type = TokenTypes.op });
        //    tokens.Add(new Token() { token = "2", type = TokenTypes.constant });
        //    tokens.Add(new Token() { token = ")" , type = TokenTypes.op });




        //    var ast = parser.Parse(tokens);
        //    parser.PrintTree(ast);
        //}

        private void Consume()
        {
            pos++;
        }

        private Token CurrentToken
        {
            get
            {
                if (pos >= Tokens.Count)
                {
                    return new Token(){tokenType = TokenTypes.eof};
                }
                return Tokens[pos];
            }
        }

        private bool Match(TokenTypes type, params string[] values)
        {
            if (CurrentToken.tokenType == type)
            {
                foreach (string val in values)
                {
                    if (CurrentToken.token.Equals(val))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void PrintTree(Node ast)
        {
            PrintTree(ast, 0);
        }

        private void PrintTree(Node n, int indent)
        {
            // Print me
            PrintWithIndent(n.Token.token, indent);

            if (n.HasChildren())
            {
                foreach (var node in n.Children())
                {
                    PrintTree(node, indent + 1); // Increase the indent for children
                }
            }
        }


        private void PrintWithIndent(string value, int indent)
        {
            Console.WriteLine("{0}{1}", new string(' ', indent * 2), value);
        }
    }
}
