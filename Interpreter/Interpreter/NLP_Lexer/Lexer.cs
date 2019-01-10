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

            // Reflects BIDMAS order of operations

            if (response.Entities.ContainsKey("identity"))
            {
                foreach(var entity in response.Entities)
                {
                    Console.WriteLine(entity);
                }
                string ident = (string) response.Entities["identity"][0]["value"];

                lex_chunk += ident;

                // Only do assignment if first character in the line is a variable, followed by equals

                if (line.Substring(0,1) == ident && line.Substring(1, 1) == "=") /*Obviously this is very hacky and should be replaced ASAP*/
                {
                    lex_chunk += " = ";
                }
            }

            if (response.Entities.ContainsKey("divide"))
            {
                if (response.Entities["number"].Count > 0)
                {
                    if (response.Entities["number"].Count == 1)
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " / ";
                        if (line.Substring(0, 1) == (string)response.Entities["identity"][0]["value"])
                        {
                            lex_chunk += response.Entities["identity"][1]["value"];
                        } else
                        {
                            lex_chunk += response.Entities["identity"][0]["value"];
                        }
                    }
                    else
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " / ";
                        lex_chunk += response.Entities["number"][1]["value"];

                    }
                }
                else
                {
                    lex_chunk += " / ";
                }
            }

            if (response.Entities.ContainsKey("multiply"))
            {
                if (response.Entities["number"].Count > 0)
                {
                    if (response.Entities["number"].Count == 1)
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " * ";
                        if (line.Substring(0, 1) == (string)response.Entities["identity"][0]["value"])
                        {
                            lex_chunk += response.Entities["identity"][1]["value"];
                        }
                        else
                        {
                            lex_chunk += response.Entities["identity"][0]["value"];
                        }
                    }
                    else
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " * ";
                        lex_chunk += response.Entities["number"][1]["value"];

                    }
                }
                else
                {
                    lex_chunk += " * ";
                }
            }

            if (response.Entities.ContainsKey("add"))
            {
                if(response.Entities["number"].Count > 0)
                {
                    if (response.Entities["number"].Count == 1)
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " + ";
                        if (line.Substring(0, 1) == (string)response.Entities["identity"][0]["value"])
                        {
                            lex_chunk += response.Entities["identity"][1]["value"];
                        }
                        else
                        {
                            lex_chunk += response.Entities["identity"][0]["value"];
                        }
                    } else
                    {
                        lex_chunk += response.Entities["number"][0]["value"];
                        lex_chunk += " + ";
                        lex_chunk += response.Entities["number"][1]["value"];

                    }
                }
                else
                {
                    lex_chunk += " + ";
                }
            }

            if (response.Entities.ContainsKey("subtract"))
            {
                // Look into intent //
                if (response.Entities["number"].Count > 0)
                {
                    if (response.Entities["number"].Count == 1)
                    {
                        lex_chunk += response.Entities["number"][1]["value"];
                        lex_chunk += " - ";
                        if (line.Substring(0, 1) == (string)response.Entities["identity"][0]["value"])
                        {
                            lex_chunk += response.Entities["identity"][1]["value"];
                        }
                        else
                        {
                            lex_chunk += response.Entities["identity"][0]["value"];
                        }
                    }
                    else
                    {
                        lex_chunk += response.Entities["number"][1]["value"];
                        lex_chunk += " - ";
                        lex_chunk += response.Entities["number"][0]["value"];

                    }
                } else
                {
                    lex_chunk += " - ";
                }
            }

            return lex_chunk;
        }
    }
}