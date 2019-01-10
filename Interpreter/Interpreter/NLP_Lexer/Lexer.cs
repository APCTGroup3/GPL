using System;
using WitAi;

namespace NLP_Lexer
{
    public class Lexer
    {
        public string ACCESS_TOKEN { get; set; }
        public Lexer(string accessToken)
        {
            this.ACCESS_TOKEN = accessToken;
        }

        public string Tokenise(string line)
        {
            Wit client = new Wit(accessToken: ACCESS_TOKEN);

            var response = client.Message(line);

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

            return lex_chunk;
        }
    }
}
