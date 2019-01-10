using System;


namespace NLP_Lexer
{
    class MainClass
    {

        static string ACCESS_TOKEN = "7DOTRBXV6DLL22FQUJRKOMSCOEUL5XG5";

        public static void Main(string[] args)
        {

            NLP_Lexer.Lexer nlpLexer = new NLP_Lexer.Lexer(ACCESS_TOKEN);

            string lex_chunk = nlpLexer.Tokenise("b = add 3 and a");
            Console.WriteLine(lex_chunk);

            //Wit client = new Wit(accessToken: ACCESS_TOKEN);

            //var response = client.Message("30.6 times 600.9");

            //string lex_chunk = string.Empty;

            //if (response.Entities.ContainsKey("add"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " + ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //if (response.Entities.ContainsKey("subtract"))
            //{
            //    // Look into intent //
            //    lex_chunk += response.Entities["number"][1]["value"];
            //    lex_chunk += " - ";
            //    lex_chunk += response.Entities["number"][0]["value"];
            //}

            //if (response.Entities.ContainsKey("divide"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " / ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //if (response.Entities.ContainsKey("multiply"))
            //{
            //    lex_chunk += response.Entities["number"][0]["value"];
            //    lex_chunk += " * ";
            //    lex_chunk += response.Entities["number"][1]["value"];
            //}

            //CoreParser.Lexer lexer = new CoreParser.Lexer(lex_chunk);

            //lexer.Tokenise();
            //List<Token> tokenList = lexer.getTokenList();
            //Console.WriteLine("Number of tokens: " + tokenList.Count);

            //foreach(var token in tokenList)
            //{
            //    Console.WriteLine(token.token);
            //}
        }
    }
}
