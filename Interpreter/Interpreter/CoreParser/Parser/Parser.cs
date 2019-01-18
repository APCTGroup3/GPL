using System;
using System.Collections.Generic;
using CoreParser.Parser.AST;

namespace CoreParser.Parser
{
    /**
     * A recursive descent parser for traversing a list of Tokens to produce a AST describing the program's structure
     */    
    public class Parser
    {
        //List of tokens in the order they should be parsed
        private IList<Token> Tokens { get; set; }
        //The root of the AST parsed from the tokens
        public Node AST { get; private set; }
        //Index of the current token
        private int pos;

        //For each opening of a self-contained code block, such as if and while blocks, maps the corresponding end token
        private readonly Dictionary<string, string> scopePairs = new Dictionary<string, string>()
        {
            {"{", "}"},
            {"if", "endif"},
            {"while", "endwhile"},
            {"for", "endfor"},
            {"else", "endelse" }
        };

        /*
         * Returns the corresponding end token for the given block-opening token (such as if or {)       
         */        
        private string GetEndScope(string startScope)
        {
            return scopePairs[startScope];
        }

           
        /*
         * Parses the given list of tokens and returns a Node corresponding to the root of the produced AST
         */
        public Node Parse(IList<Token> tokens)
        {
            //Reset fields
            Tokens = tokens;
            pos = 0;

            AST = ParseStatements();
            //PrintTree(AST);
            return AST;

        }


        //Parses a block of statements. This should be the entry point for the parser
        // if a startScope value is provided, corresponding to the start of a self-contained code block such as if or {,
        // statements will be added to the blockNode until a corresponding end of scope token is found.
        // If this is not set, or is null, statements will be added until the end the token list.
        private Node ParseStatements(string startScope)
        {
            //Create new node
            BlockNode node = new BlockNode(CurrentToken);
            List<Node> statements = new List<Node>();
            bool inScope = true;
            while (inScope)
            {
                //Check if block is complete
                if (startScope == null)
                {
                    inScope = CurrentToken.tokenType != TokenTypes.eof;
                }
                else
                {
                    inScope = CurrentToken.token != GetEndScope(startScope);
                }

                if (!inScope) //Remove end-of-scope token and finish
                {
                    Consume();
                    break;
                }

                //Else process next statement and add to list
                if (CurrentToken.tokenType == TokenTypes.newline)
                {
                    Consume();
                }
                else if (CurrentToken.token == "{")
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

        //Parses a single statement and returns corresponding node.
        // This part of the parser will continue to descend through other parse functions in descending order of precedence
        // until a matching rule is hit.
        private Node ParseStatement()
        {
            //Handle if statements and while loops
            if (CurrentToken.tokenType == TokenTypes.keyword)
            {
                switch(CurrentToken.token)
                {
                    case "if":
                        return ParseIf();
                    case "while":
                        return ParseWhile();
                    case "do":
                        Consume();
                        return ParseStatement();
                    default:
                        throw new Exception(CurrentToken.token + " not implemented yet");
                }
            }
            else //Continue
            {
                return ParseAssignment();
            }
        }


        //Parses an if statement
        private Node ParseIf()
        {
            var token = CurrentToken;
            
            Consume(); //Consume if/else/elif token
            Node condition = ParseExpression();
            Node statements = null;
            if (CurrentToken.token == "then") //ignore optional then keyword
            {
                Consume();
            }
            //Check if inline if or block
            if (CurrentToken.tokenType == TokenTypes.newline) //Continue until endif unless next token is {
            {
                while (CurrentToken.tokenType == TokenTypes.newline) //ignore newlines
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

        //Parses else and elseif/elif statements. 'Else ifs' are handled as a series of nested if statements.
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
            var tokenStr = CurrentToken.token.ToLower(); //ignore keyword case
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
                Node condition = ParseExpression();
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

        //Parses a variable or array assignment statement, else continues.
        private Node ParseAssignment()
        {
            //Statement can only be assignment if first token is a valid variable ID.
            if (CurrentToken.tokenType == TokenTypes.identity)
            {
                try
                {
                    //If next token is valid assignment operator or array element then assume assignment
                    if (Tokens[pos + 1].tokenType == TokenTypes.op && (Tokens[pos + 1].token.Equals("=") || Tokens[pos + 1].token.Equals("[")))
                    {
                        var id = ParseVarName();
                        if (CurrentToken.token.Equals("[")) //Array element assignment
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

                            if (CurrentToken.token.Equals("]")) //Must match opening and closing []
                            {
                                Consume();
                            }
                            else
                            {
                                throw new ParserException("Expected ], found " + CurrentToken.token);
                            }

                            //Allow variety of assignment symbols
                            if (CurrentToken.token.Equals("=") || CurrentToken.token.Equals("<-") || CurrentToken.token.Equals(":=")) 
                            {
                                Consume();
                            }
                            else
                            {
                                throw new ParserException("Expected assignment operator, found " + CurrentToken.token);
                            }

                            //RHS of assignment statement, must be either expression of variable value
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
                        else //Variable assignment
                        {
                            var node = new Assignment(CurrentToken);
                            //Allow variety of assignment symbols
                            if (CurrentToken.token.Equals("=") || CurrentToken.token.Equals("<-") || CurrentToken.token.Equals(":="))
                            {
                                Consume();
                            }
                            else
                            {
                                throw new ParserException("Expected assignment operator, found " + CurrentToken.token);
                            }
                            Node value = null;
                            try //RHS
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
                    else // Not assignment, assume expression or function
                    {
                        if (Tokens[pos + 1].token.Equals("(")) // Assume method
                        {
                            return ParseVarOrFunction();
                        }
                        return ParseExpression();
                    }
                }
                catch (Exception)
                {
                    if (Tokens[pos + 1].token.Equals("(")) // Assume method
                    {
                        return ParseVarOrFunction();
                    }
                    return ParseExpression();
                }
            }
            else //not assignment, try boolean
            {
                return ParseEquality();
            }
        }

        //Parses a variable name or function call, else continues
        private Node ParseVarOrFunction()
        {
            Node name = ParseVarName();
            if (CurrentToken.token.Equals("(")) //Assume function
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
                //Parse index of element, either numerical expression,  variable or function call
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
            return new Variable(name.Token); //Else variable
        }


        //Parses the name of a variable, or throws an exception is token is not valid ID
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

        //Entry point for parsing of numerical expressions.
        //Descencs through mathematical operations of decreasing precedence until match is found.
        private Node ParseExpression()
        {
            return ParseEquality(); //Highest operator precedence
        }

        //Parses boolean equality operator, else continues to non-equal boolean comparison
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

        //Parses non-equal boolean comaprisons or continues to Addition
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

        //Parses addition or continues to multiplication
        private Node ParseAdd()
        {
            var left = ParseMult();
            if (Match(TokenTypes.keyword, "add", "plus", "subtract")  || Match(TokenTypes.op, "+", "-"))
            {
                if (Match(TokenTypes.op, "+") || Match(TokenTypes.keyword, "add", "plus"))
                {
                    CurrentToken.token = "+";
                }
                else if (Match(TokenTypes.op, "-") || Match(TokenTypes.keyword, "subtract"))
                {
                    CurrentToken.token = "-";
                }
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

        //Parses multiplication/diviion or continues to power function
        private Node ParseMult()
        {
            var left = ParsePower();
            if (Match(TokenTypes.op, "*") || Match(TokenTypes.keyword, "mult", "multiply"))
            {
                CurrentToken.token = "*";
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParseMult();
                return node;
            }
            else if (Match(TokenTypes.op, "/") || Match(TokenTypes.keyword,  "div", "divide"))
            {
                CurrentToken.token = "/";
                var node = new BinaryOp(CurrentToken);
                node.Left = left;
                Consume();
                node.Right = ParseMult();
                return node;
            }
            {
                return left;
            }
        }

        //Parses power operator or continues to bracketed expression
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

        //Parses mathematical expression contained within brackets, or continues to negative function
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

        //Parses unary negative operation, else tries parsing terminal
        private Node ParseNegative()
        {
            if (Match(TokenTypes.op, "-", "minus"))
            {
                CurrentToken.token = "-";
                var node = new UnaryNode(CurrentToken);
                Consume();
                node.Child = ParseExpression();
                return node;
            }
            else
            {
                return ParseNot();
            }
        }


        //Parses boolean negation
        private Node ParseNot()
        {
            if (Match(TokenTypes.op, "!", "not"))
            {
                var node = new UnaryNode(new Token() { token = "!"});
                Consume();
                node.Child = ParseExpression();
                return node;
            }
            else
            {
                return ParseFactor();
            }
        }

        //Parses terminal value, else throws an exception.
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

        //Parses boolean, or throws an exception
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

        //Advances to the next token in the list
        private void Consume()
        {
            pos++;
        }

        //Returns the token at the current position
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

        //Returns true if the current token has the same token type as the given type,
        // and its token value matches at least one of the given values
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

        /* Debugging functions for printing the parse tree to the console
         */       
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
