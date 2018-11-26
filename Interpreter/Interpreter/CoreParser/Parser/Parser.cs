using System;
using System.Collections.Generic;
using CoreParser.Parser.AST;

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

            AST = ParseStatements();
            return AST;

        }

        /****** NUMERIC EXPRESSIONS *******/
        // Any mathematical expression that returns a number

        private Node ParseStatements()
        {
            BlockNode node = new BlockNode(new Token());
            List<Node> statements = new List<Node>();
            while (CurrentToken.tokenType != TokenTypes.eof)
            {
                if (CurrentToken.tokenType == TokenTypes.newline)
                {
                    Consume();
                }
                else
                {
                    statements.Add(ParseStatement());
                }
            }
            node.Statements = statements;
            return node;
        }

        private Node ParseStatement()
        {
            if (CurrentToken.tokenType == TokenTypes.statement)
            {
                switch(CurrentToken.token)
                {
                    case "if":
                        return ParseIf();
                    default:
                        throw new Exception(CurrentToken.token + " not implemented yet");
                }
            }
            else
            {
                return ParseAssignment();
            }
        }

        private Node ParseIf()
        {
            if (CurrentToken.tokenType == TokenTypes.statement && CurrentToken.token == "if")
            {
                Consume();
                Node condition = ParseExpression();
                if (CurrentToken.token == "then")
                {
                    Consume();
                    Node statements = ParseStatements();
                    Node node = new IfNode(new Token(), condition, statements);
                    return node;
                }
            }
            throw new ParserException("Expected \"if\", found " + CurrentToken.token);
        }

        private Node ParseVarOrFunction()
        {
            Node name = ParseVarName();
            if (CurrentToken.token.Equals("("))
            {
                Consume();
                List<Node> parameters = new List<Node>();
                while (!CurrentToken.token.Equals(")")) 
                {
                    parameters.Add(ParseExpression());
                }
                Consume(); //Consume closing )
                FunctionNode node = new FunctionNode(name.Token);
                node.Parameters = parameters;
                return node;
            }
            return new Variable(name.Token);
        }

        private Node ParseAssignment()
        {
            if (CurrentToken.tokenType == TokenTypes.identity)
            {
                try
                {
                    if (Tokens[pos + 1].tokenType == TokenTypes.op && Tokens[pos + 1].token.Equals("="))
                    {
                        var id = ParseVarName();
                        var node = new Assignment(CurrentToken);
                        Consume(); //Consume 
                        Node value = null;
                        try
                        {
                            value = ParseExpression();
                        }
                        catch (Exception e)
                        {
                            value = ParseVarOrFunction();
                        }
                        node.ID = id;
                        node.Value = value;
                        return node;
                    }
                    else
                    {
                        if (Tokens[pos + 1].token.Equals("(")) // Assume method
                        {
                            return ParseVarOrFunction();
                        }
                        return ParseExpression();
                    }
                } catch (Exception e)
                {
                    if (Tokens[pos+1].token.Equals("(")) // Assume method
                    {
                        return ParseVarOrFunction();
                    }
                    return ParseExpression();
                }
            }
            else
            {
                return ParseEquality();
            }
        }

        private Node ParseVarName()
        {
            var valid = CurrentToken.tokenType == TokenTypes.identity;
            if (!valid)
            {
                throw new ParserException("Line " + CurrentToken.lineNumber + ": Expected identifier; found " + CurrentToken.token);
            }
            else
            {
                var node = new TerminalNode(CurrentToken);
                Consume();
                return node;
            }
        }

        private Node ParseExpression()
        {
            return ParseEquality();
        }

        private Node ParseEquality()
        {
            var left = ParseComparison();
            if (Match(TokenTypes.op, "==", "eq", "equals", "!=", "neq", "notequ"))
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


        private Node ParseComparison()
        {
            var left = ParseAdd();
            if (Match(TokenTypes.op, "<", "lt", "lessthan", "<=", "lte", "lessthanequal", ">", "gt", "greaterthan", ">=", "gte", "greaterthanequal"))
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
            if (CurrentToken.tokenType == TokenTypes.constant)
            {
                var node = new TerminalNode(CurrentToken);
                Consume();
                return node;  
            } 
            else if (CurrentToken.tokenType == TokenTypes.identity)
            {
                return ParseVarOrFunction(); 
            }
            else
            {
                throw new ParserException("Line " + CurrentToken.lineNumber + ": Unexpected value; found " + CurrentToken.token);
            }
        }

        private Node ParseBool()
            {
                var valid = CurrentToken.tokenType == TokenTypes.boolean;
                if (!valid)
                {
                    throw new ParserException("Line " + CurrentToken.lineNumber + ": Expected boolean; found " + CurrentToken.token);
                }
                else
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
