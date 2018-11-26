using System;
using System.Collections.Generic;
using WitAi;
using CoreParser;


namespace NLP
{
    class MainClass
    {

        static string ACCESS_TOKEN = "ACCESS_TOKEN";

        public static void Main(string[] args)
        {
            Wit client = new Wit(accessToken: ACCESS_TOKEN);

            var response = client.Message("30.6 times 600.9");

            string lex_chunk = string.Empty;

            if (response.Entities.ContainsKey("add"))
            {
                lex_chunk += response.Entities["number"][0]["value"];
                lex_chunk += " + ";
                lex_chunk += response.Entities["number"][1]["value"];
            }

            if (response.Entities.ContainsKey("subtract"))
            {
                // Look into intent //
                lex_chunk += response.Entities["number"][1]["value"];
                lex_chunk += " - ";
                lex_chunk += response.Entities["number"][0]["value"];
            }

            if (response.Entities.ContainsKey("divide"))
            {
                lex_chunk += response.Entities["number"][0]["value"];
                lex_chunk += " / ";
                lex_chunk += response.Entities["number"][1]["value"];
            }

            if (response.Entities.ContainsKey("multiply"))
            {
                lex_chunk += response.Entities["number"][0]["value"];
                lex_chunk += " * ";
                lex_chunk += response.Entities["number"][1]["value"];
            }

            CoreParser.Lexer lexer = new CoreParser.Lexer(lex_chunk);

            lexer.Tokenise();
            List<Token> tokenList = lexer.getTokenList();
            Console.WriteLine("Number of tokens: " + tokenList.Count);

            foreach(var token in tokenList)
            {
                Console.WriteLine(token.token);
            }
        }
    }
}
