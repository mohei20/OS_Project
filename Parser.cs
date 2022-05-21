using System;
using System.Collections.Generic;

namespace OS_Project
{
    public static class Parser
    {
        public static void parse(string input)
        {
            List<Token> tokens = Tokenizer.Get_Tokens(input);
            if (tokens == null)
                return;
            if (tokens[0].key == Type_Of_Token.Not_Recognized)
                Console.WriteLine(tokens[0].value + " is not recognized as an internal or external command,\n operable program or batch file.");
            else if (tokens[0].key == Type_Of_Token.Command)
            {
                switch (tokens[0].value)
                {
                    case "cd":
                        Commands.Change_Directory(tokens);
                        break;
                    case "cls":
                        Commands.Clear_Screen(tokens);
                        break;
                    case "dir":
                        Commands.List_Directory(tokens);
                        break;
                    case "help":
                        Commands.help(tokens);
                        break;
                    case "md":
                        Commands.Create_Directory(tokens);
                        break;
                    case "quit":
                        Commands.quit(tokens);
                        break;
                    case "rd":
                        Commands.Remove_Directory(tokens);
                        break;
                }
            }
        }
    }
}

