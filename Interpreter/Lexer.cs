namespace Interpreter{

    class Lexer{
        /* PRIVATE */
        private List<Token> tokenList;
        private string sourceCode;

        private List<Regex> tokenDefinitions = new List<Regex>();

        private tokenTypes findTokenType(string tokenCode){

            /* https://jack-vanlightly.com/blog/2016/2/3/creating-a-simple-tokenizer-lexer-in-c */



        }

        /* PUBLIC */
        public Lexer(string sourceCode){
            this.sourceCode = sourceCode;

            tokenDefinitions.Add(new Regex(@"AND|&|and|&&"));
            tokenDefinitions.Add(new Regex(@"OR|or|\|\|"));
            tokenDefinitions.Add(new Regex(@"PLUS|\+|plus|add|ADD"));
            tokenDefinitions.Add(new Regex(@"\-|minus|sub|SUB"));
            tokenDefinitions.Add(new Regex(@"then|THEN"));

        }

        public Token[] tokenise(){
            Token[] tokens;
            
            /* Do tokenisation... */
            foreach(Regex definition in tokenDefinitions){

                MatchCollection matches = definition.Matches(this.sourceCode);
                foreach(Match match in matches){
                    if(match.Success){
                        //Find the token's type
                        // Create the token and add token to token list
                        Console.WriteLine(match.Value);
                    }
                }
            }


            return tokens;
        }
        public void setSourceCode(string sourceCode){
            this.sourceCode = sourceCode;
        }

        public Token[] getTokenList(){
            return this.tokenList;
        }
    }
}



/*


switch(tokenCode){
                case "if":
                    return tokenTypes.conditional;
                case "else":
                    return tokenTypes.conditional;
                case "+":
                    return tokenTypes.expression;
                case "-":
                    return tokenTypes.expression;
                case "/":
                    return tokenTypes.expression;
                case "*":
                    return tokenTypes.expression;
                case "++":
                    return tokenTypes.expression;
                case "--":
                    return tokenTypes.expression;
                case "^":
                    return tokenTypes.expression;
                case "=":
                    return tokenTypes.expression;
                case "==":
                    return tokenTypes.expression;
                case "&":
                    return tokenTypes.expression;
                case "&&":
                    return tokenTypes.expression;
                case "||":
                    return tokenTypes.expression;
                case "%":
                    return tokenTypes.expression;
                default:
                    return tokenTypes.identity;
            }


*/