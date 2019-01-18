using System;
using WitAi;

namespace NLP_Lexer
{
    public class Lexer
    {
        public string ACCESS_TOKEN { get; set; }

        // Set access token on object creation
        public Lexer(string accessToken)
        {
            this.ACCESS_TOKEN = accessToken;
        }

        public string Tokenise(string line)
        {
            // Create a new wit client to connect to wit.ai
            Wit client = new Wit(accessToken: ACCESS_TOKEN);

            // Get wit response
            var response = client.Message(line);


            string lex_chunk = string.Empty;

            // Reflects BIDMAS order of operations

            if (response.Entities.ContainsKey("identity"))
            {
                // Deal with identities
                string ident = (string) response.Entities["identity"][0]["value"];

                // Add to lex_chunk
                lex_chunk += ident;

                // Only do assignment if first character in the line is a variable, followed by equals
                if (line.Substring(0,1) == ident && (line.Substring(1, 1) == "=" || line.Substring(1, 2) == " =" || line.Substring(1, 2) == " = ") )
                {
                    lex_chunk += " = ";
                }
            }

            if (response.Entities.ContainsKey("divide"))
            {
                // Divide 

                // Outputs x / y
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
                // Multiply 
                // output x * y
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
                // Add 
                // outpus x + y
                if (response.Entities["number"].Count > 0)
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
                // Subtract 
                // outputs x - y
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

            if (response.Entities.ContainsKey("function"))
            {
                // Deal with functions
                var function_name = response.Entities["function"][0]["value"];
                lex_chunk = lex_chunk.Insert(0, function_name + "(");
                lex_chunk += ")";
            }

            return lex_chunk;
        }
    }
}