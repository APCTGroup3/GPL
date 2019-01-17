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

        private readonly Dictionary<string, string> scopePairs = new Dictionary<string, string>()
        {
            {"{", "}"},
            {"if", "endif"},
            {"while", "endwhile"},
            {"for", "endfor"},
            {"else", "endelse" }
        };

        private string GetEndScope(string startScope)
        {
            return scopePairs[startScope];
        }

        public Node Parse(IList<Token> tokens)
        {
            Tokens = tokens;
            pos = 0;

            AST = ParseStatements();
            return AST;

        }

        /****** NUMERIC EXPRESSIONS *******/
        // Any mathematical expression that returns a number

        private Node ParseStatements(string startScope)
        {

            BlockNode node = new BlockNode(CurrentToken);
            List<Node> statements = new List<Node>();
            //if (startScope != null)
            bool inScope = true;
            while (inScope)
            {
                if (startScope == null)
                {
                    inScope = CurrentToken.tokenType != TokenTypes.eof;
                }
                else
                {
                    inScope = CurrentToken.token != GetEndScope(startScope);
                }

                if (!inScope)
                {
                    Consume();
                    break;
                }

                if (CurrentToken.tokenType == TokenTypes.newline)
                {
                    Consume();
                }
                else if (CurrentToken.token == "{") //Start new scope
                {
                    statements.Add(ParseStatements("{"));
                }
                else
                { 
                    statements.Add(ParseStatement());
                }
           }
           node.Statements = statements;
           return node;
        }

        private Node ParseStatements()
        {
            return ParseStatements(null);
        }

        private Node ParseStatement()
        {
            if (CurrentToken.tokenType == TokenTypes.keyword)
            {
                switch(CurrentToken.token)
                {
                    case "if":
                        return ParseIf();
                    case "while":
                        return ParseWhile();
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
            var token = CurrentToken;
            
            Consume();
            Node condition = ParseExpression();
            Node statements = null;
            if (CurrentToken.token == "then")
            {
                Consume();
            }
            //Check if inline if or block
            if (CurrentToken.tokenType == TokenTypes.newline) //Continue until endif unless next token is {
            {
                while (CurrentToken.tokenType == TokenTypes.newline)
                {
                    Consume();
                }
                if (CurrentToken.token == "{") //Continue until }
                {
                    Consume(); //{
                    statements = ParseStatements("{");
                }
                else //Continue until endif
                { 
                    statements = ParseStatements("if");
                }
            }
            else
            {
                if (CurrentToken.token == "{") //Continue until }
                {
                    Consume(); //{
                    statements = ParseStatements("{");
                }
                else //inline if
                {
                    statements = ParseStatement();
                }
            }

            //Check for else statement
            Node elseStatements = ParseElse();

            Node node = (elseStatements == null) ? new IfNode(token, condition, statements) : new IfNode(token, condition, statements, elseStatements);
                return node;
        }

        private Node ParseElse()
        {
            //Ignore newlines
            if (CurrentToken.tokenType == TokenTypes.newline)
            {
                while (CurrentToken.tokenType == TokenTypes.newline)
                {
                    Consume();
                }
            }
            var tokenStr = CurrentToken.token.ToLower();
            if (tokenStr == "else")
            {
                Consume();
                if (CurrentToken.tokenType == TokenTypes.newline) //Continue until endif unless next token is {
                {
                    while (CurrentToken.tokenType == TokenTypes.newline)
                    {
                        Consume();
                    }
                    if (CurrentToken.token == "{") //Continue until }
                    {
                        Consume(); //{
                        return ParseStatements("{");
                    }
                    else //continue until endif
                    {
                        return ParseStatements("else");
                    }
                }
                else if (CurrentToken.token == "{") //Continue until }
                {
                    Consume(); //{
                    return ParseStatements("{");
                }
                else //inline else or else if
                {
                    return ParseStatement();
                }

            }
            else if (tokenStr == "elif" || tokenStr == "elseif")
            {
                return ParseIf();
            }
            else
                return null;
        }

        private Node ParseWhile()
        {
            if (CurrentToken.tokenType == TokenTypes.keyword && CurrentToken.token == "while")
            {
                var token = CurrentToken;

                Consume();
                Expression condition = (Expression)ParseExpression();
                Node statements = null;
                if (CurrentToken.token == "do")
                {
                    Consume();
                }
                //Check if inline if or block
                if (CurrentToken.tokenType == TokenTypes.newline) //Continue until endif unless next token is {
                {
                    while (CurrentToken.tokenType == TokenTypes.newline)
                    {
                        Consume();
                    }
                    if (CurrentToken.token == "{") //Continue until }
                    {
                        Consume(); //{
                        statements = ParseStatements("{");
                    }
                    else //Continue until endif
                    {
                        statements = ParseStatements("while");
                    }
                }
                else
                {
                    if (CurrentToken.token == "{") //Continue until }
                    {
                        Consume(); //{
                        statements = ParseStatements("{");
                    }
                    else //inline if
                    {
                        statements = ParseStatement();
                    }
                }
                Node node = new WhileNode(token, condition, statements);
                return node;
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
                    if (CurrentToken.token.Equals(","))
                    {
                        Consume();
                    }
                }
                Consume(); //Consume closing )
                FunctionNode node = new FunctionNode(name.Token);
                node.Parameters = parameters;
                return node;
            }
            else if (CurrentToken.token.Equals("[")) //Array element
            {
                Consume();
                Node element;
                try
                {
                    element = ParseExpression();
                }
                catch (Exception)
                {
                    element = ParseVarOrFunction();
                }
                if (CurrentToken.token.Equals("]"))
                {
                    Consume();
                } 
                else
                {
                    throw new ParserException("Expected ], found " + CurrentToken.token);
                }
                return new ArrayElement(name.Token, element);
            }
            return new Variable(name.Token);
        }

        private Node ParseAssignment()
        {
            if (CurrentToken.tokenType == TokenTypes.identity)
            {
                try
                {
                    if (Tokens[pos + 1].tokenType == TokenTypes.op && (Tokens[pos + 1].token.Equals("=") || Tokens[pos + 1].token.Equals("[")))
                    {
                        var id = ParseVarName();
                        if (CurrentToken.token.Equals("[")) //Array
                        {
                            var node = new ArrayAssignment(CurrentToken);
                            Consume(); //Consume [
                            Node value = null;
                            Node element = null;
                            try
                            {
                                element = ParseExpression();
                            }
                            catch (Exception)
                            {
                                element = ParseVarOrFunction();
                            }

                            if (CurrentToken.token.Equals("]"))
                            {
                                Consume();
                            }
                            else
                            {
                                throw new ParserException("Expected ], found " + CurrentToken.token);
                            }

                            if (CurrentToken.token.Equals("=") || CurrentToken.token.Equals("<-") || CurrentToken.token.Equals(":="))
                            {
                                Consume();
                            }
                            else
                            {
                                throw new ParserException("Expected =, found " + CurrentToken.token);
                            }

                            try
                            {
                                value = ParseExpression();
                            }
                            catch (Exception)
                            {
                                value = ParseVarOrFunction();
                            }

                            node.ID = id;
                            node.Value = value;
                            node.Element = element;
                            return node;
                        }
                        else
                        {
                            var node = new Assignment(CurrentToken);
                            Consume(); //Consume =
                            Node value = null;
                            try
                            {
                                value = ParseExpression();
                            }
                            catch (Exception)
                            {
                                value = ParseVarOrFunction();
                            }
                            node.ID = id;
                            node.Value = value;
                            return node;
                        }
                    }
                    else
                    {
                        if (Tokens[pos + 1].token.Equals("(")) // Assume method
                        {
                            return ParseVarOrFunction();
                        }
                        return ParseExpression();
                    }
                } catch (Exception)
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
                    return new Token(){tokenType = TokenTypes.eof, token="EOF"};
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
