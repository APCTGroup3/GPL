using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreParser
{
    public class Lexer
    {

        public List<Token> TokenList { get; private set; }
        private string sourceCode;

        //Reserved words
        private static string[] reservedKeywords =
        {
            "for", "do", "if", "else", "then", "while", "elif"
        };
        private static string[] reservedOps =
        {
            "and", "or"
        };
        private static string[] reservedBools =
        {
            "true", "false"
        };

        private static string symbols = "+-=&|/*(){}<>[]!%.,?;:^";
        private const char END_OF_FILE = '\0';
        private const char NEWLINE = '\n';

        //Vars for traversing source code
        private int lineNum;
        private int colNum;
        private int position;
        private string currentToken; //Token being built


        //Accessors for chars within soursecode
        private char Current
        {
            get
            {
                return Peek(0);
            }
        }
        private char Next
        {
            get
            {
                return Peek(1);
            }
        }
        private char Previous
        {
            get
            {
                return Peek(-1);
            }
        }

        private char Peek(int charsAhead)
        {
            if (position + charsAhead >= sourceCode.Length)
            {
                return '\0';
            }
            return sourceCode[position + charsAhead];
        }

        public Lexer(string sourceCode)
        {
            this.sourceCode = sourceCode;
            TokenList = new List<Token>();
        }

        private void Consume()
        {
            currentToken += Current;
            Advance();
        }
        private void Advance()
        {
            position++;
            colNum++;
        }
        private void ProcessNewLine()
        {
            lineNum++;
            colNum = 0;
        }


        public void Tokenise() {
            //Reset params
            TokenList = new List<Token>();
            lineNum = 1;
            position = 0;
            colNum = 0;
            currentToken = "";

            while (!IsEndOfFile() || position < sourceCode.Length)
            {
                ProcessNextToken();
            }
            //Make sure EOF is final token
            BuildToken(TokenTypes.eof);
        }

        private void ProcessNextToken()
        {
            //Attempt to create tokens depending on current char
            if (IsEndOfFile()) //If file is empty
            {
                BuildToken(TokenTypes.eof);
            }
            else if (IsComment())
            {
                ScanComment();
            }
            else if (IsNewLine())
            {
                ScanNewLine();
            }
            else if (IsWhitespace())
            {
                ScanWhitespace();
            }
            else if (IsDigit())
            {
                ScanNumber();
            }
            else if (IsIdentifierStart())
            {
                ScanIdentifier();
            }
            else if (isSymbol())
            {
                ScanSymbol();
            }
            else if (Current == '"' || Current == '\'')
            {
                ScanString(Current);
            }
        }

        //Create new token and add to list
        private void BuildToken(TokenTypes type)
        {
            Token token = new Token
            {
                token = currentToken.ToString(),
                lineNumber = lineNum,
                tokenType = type
            };
            TokenList.Add(token);
            currentToken = "";
        }
        private void BuildToken(TokenTypes type, ConstTypes constType)
        {
            Token token = new Token
            {
                token = currentToken.ToString(),
                lineNumber = lineNum,
                tokenType = type,
                constType = constType
            };
            TokenList.Add(token);
            currentToken = "";
        }


        /* Methods for identifying character type */
        private bool IsDigit()
        {
            return char.IsDigit(Current);
        }
        private bool IsEndOfFile()
        {
            return Current < 0 || Current == END_OF_FILE;
        }
        private bool IsNewLine()
        {
            return Current == NEWLINE;
        }
        private bool IsIdentifierStart()
        {
            return Current == '_' || IsLetter();
        }
        private bool IsIdentifier()
        {
            return IsIdentifierStart() || IsDigit();
        }
        private bool IsLetter()
        {
            return char.IsLetter(Current);
        }
        private bool IsLetterOrDigit()
        {
            return char.IsLetterOrDigit(Current);
        }
        private bool IsWhitespace()
        {
            return !IsNewLine() && (char.IsWhiteSpace(Current) || IsEndOfFile());
        }
        private bool isSymbol()
        {
            return symbols.Contains(Current);
        }
        private bool IsComment()
        {
            if (Current == '/')
            {
                return Next == '/' || Next == '*';
            }
            else if (Current == '#')
            {
                return true;
            }
            return false;
        }

        //Checks if in-progress token is a reserved word
        private bool IsReservedWord()
        {
            return reservedKeywords.Contains(currentToken.ToLower())
                || reservedOps.Contains(currentToken.ToLower())
                || reservedBools.Contains(currentToken.ToLower());
        }


        //Methods for scanning and creating tokens
        private void ScanComment()
        {
            if(Next == '/' || Current == '#') //Line comment
            { 
                //Ignore everything until new line
                while (!IsNewLine() && !IsEndOfFile())
                {
                    Advance();
                }
            }
            else if (Next == '*') // /* */ Block Comment
            {
                while (!(Current == '*' && Next == '/'))
                {
                    Advance();
                }
                Advance();
                Advance();
            }
        }
        private void ScanNewLine()
        {
            Consume();
            ProcessNewLine();
            BuildToken(TokenTypes.newline);
        }
        private void ScanWhitespace()
        {
            //Skip over whitespace
            while (IsWhitespace())
            {
                Advance();
            }
        }
        private void ScanNumber()
        {
            while(IsDigit())
            {
                Consume();
            }
            //If decimal point and digit follows, treat as float, otherwise token is an int
            if (Current == '.' && char.IsDigit(Next))
            {
                Consume();
                while(IsDigit())
                {
                    Consume();
                }
            }
            BuildToken(TokenTypes.constant, ConstTypes.number);
        }
        private void ScanIdentifier()
        {
            while(IsIdentifier())
            {
                Consume();
            }

            //Check if reserved word
            if (IsReservedWord())
            {
                if (reservedKeywords.Contains(currentToken.ToLower()))
                {
                    BuildToken(TokenTypes.keyword);
                }
                else if (reservedOps.Contains(currentToken.ToLower()))
                {
                    BuildToken(TokenTypes.op);
                }
                else if (reservedBools.Contains(currentToken.ToLower()))
                {
                    BuildToken(TokenTypes.constant, ConstTypes.boolean);
                }
            }
            else
            {
                BuildToken(TokenTypes.identity);
            }
        }
        private void ScanSymbol()
        {
            switch (Current)
            {
                case '+':
                case '-':
                case '/':
                case '*':
                case '^':
                case '.':
                case ',':
                case '(':
                case ')':
                case '{':
                case '}':
                case '[':
                case ']':
                case ';':
                    Consume(); 
                    BuildToken(TokenTypes.op);
                    break;
                case '<':
                    Consume();
                    if (Current == '=' || Current == '-')
                        Consume();
                    BuildToken(TokenTypes.op);
                    break;
                case '>':
                case ':':
                case '!':
                case '=':
                    Consume();
                    if (Current == '=')
                        Consume();
                    BuildToken(TokenTypes.op);
                    break;

                case '&':
                    //Allow & and && as and symbol
                    Consume();
                    if (Current == '&')
                    {
                        Consume();
                    }
                    BuildToken(TokenTypes.op);
                    break;

                case '|':
                    //Sim for OR
                    Consume();
                    if (Current == '|')
                    {
                        Consume();
                    }
                    BuildToken(TokenTypes.op);
                    break;

                default:
                    throw new Exception("Unexpected symbol " + Current);
            }
        }
        private void ScanString(char delimiter)
        {
            Advance();
            while (Current != delimiter)
            {
                //Allow multi-line string but don't ignore newlines as newlines
                if (IsEndOfFile())
                {
                    throw new Exception("Encountered end of file");
                }
                else
                {
                    Consume();
                }
            }
            Advance(); //Past delimiter
            BuildToken(TokenTypes.constant, ConstTypes.str);
        }

        public List<Token> getTokenList()
        {
            return TokenList;
        }
    }
}
