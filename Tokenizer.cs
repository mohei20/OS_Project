using System.Collections.Generic;
using System.Linq;

namespace OS_Project
{
    public static class Tokenizer
    {
//---------------------------------------------------------------------------------------------------------------------------//
        private static Token Generate_Token(string arg, Type_Of_Token tokenType)
        {
            Token token;
            token.key = tokenType;
            token.value = arg;
            return token;
        }
//---------------------------------------------------------------------------------------------------------------------------//
        private static bool Check_Arg(string arg)
        {
            if (arg == "cd" || arg == "cls" || arg == "dir" || arg == "quit" || arg == "copy" || arg == "del" || arg == "help" || arg == "md" || arg == "rd" || arg == "rename" || arg == "type" || arg == "import" || arg == "export")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
//--------------------------------------------------------------------------------------------------------------------------------------//
        private static bool Is_FullPath_To_dir(string arg) => (arg.Contains(":") || arg.Contains("\\")) && !arg.Contains<char>('.');
//--------------------------------------------------------------------------------------------------------------------------------------//
        private static bool Is_FullPath_To_File(string arg) => (arg.Contains(":") || arg.Contains("\\")) && arg.Contains<char>('.');
//---------------------------------------------------------------------------------------------------------------------------------------//
        private static bool Is_Fulle_Name(string arg) => arg.Contains<char>('.');
//-----------------------------------------------------------------------------------------------------------------------------------------//
        public static List<Token> Get_Tokens(string input)
        {
            List<Token> tokens = new List<Token>();

            if (input.Length == 0)
                return (List<Token>)null;

            string[] strArray = input.Split(' ');

            List<string> strList = new List<string>();

            for (int index = 0; index < strArray.Length; ++index)
            {
                if (strArray[index] != "" && strArray[index] != " ")
                    strList.Add(strArray[index]);
            }

            string[] array = strList.ToArray();

            array[0] = array[0].ToLower();
            switch (array[0])
            {
                case "cd":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command)); //cd command
                        break;
                    }
                    if (array.Length == 2)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        if (Tokenizer.Is_FullPath_To_dir(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_Directory));
                            break;
                        }
                        if (Tokenizer.Is_FullPath_To_File(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_File));
                            break;
                        }
                        if (Tokenizer.Is_Fulle_Name(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.File_Name));
                            break;
                        }
                        tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.DirName));
                        break;
                    }

                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));

                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;

                case "cls":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    if (array.Length > 1)
                    {
                        for (int index = 1; index < array.Length; ++index)
                            tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                        break;
                    }
                    break;

                case "copy":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;

                case "del":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;

                case "dir":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        break;
                    }

                    if (array.Length == 2)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        if (Tokenizer.Is_FullPath_To_dir(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_Directory));
                            break;
                        }
                        if (Tokenizer.Is_FullPath_To_File(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_File));
                            break;
                        }
                        if (Tokenizer.Is_Fulle_Name(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.File_Name));
                            break;
                        }
                        tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.DirName));
                        break;
                    }

                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));

                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;

                case "export":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;

                case "help":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        break;
                    }
                    if (array.Length == 2)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        array[1] = array[1].ToLower();
                        if (Tokenizer.Check_Arg(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.Command));
                            break;
                        }
                        tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.Not_Recognized));
                        break;
                    }
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;
                case "import":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;
                case "md":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        break;
                    }
                    if (array.Length == 2)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        if (Tokenizer.Is_FullPath_To_dir(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_Directory));
                            break;
                        }
                        if (Tokenizer.Is_FullPath_To_File(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_File));
                            break;
                        }
                        if (Tokenizer.Is_Fulle_Name(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.File_Name));
                            break;
                        }
                        tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.DirName));
                        break;
                    }
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;
                case "quit":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        break;
                    }
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;
                case "rd":
                    if (array.Length == 1)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        break;
                    }
                    if (array.Length == 2)
                    {
                        tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                        if (Tokenizer.Is_FullPath_To_dir(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_Directory));
                            break;
                        }
                        if (Tokenizer.Is_FullPath_To_File(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.FullPath_To_File));
                            break;
                        }
                        if (Tokenizer.Is_Fulle_Name(array[1]))
                        {
                            tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.File_Name));
                            break;
                        }
                        tokens.Add(Tokenizer.Generate_Token(array[1], Type_Of_Token.DirName));
                        break;
                    }
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    for (int index = 1; index < array.Length; ++index)
                        tokens.Add(Tokenizer.Generate_Token(array[index], Type_Of_Token.Not_Recognized));
                    break;
                case "rename":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;
                case "type":
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Command));
                    break;
                default:
                    tokens.Add(Tokenizer.Generate_Token(array[0], Type_Of_Token.Not_Recognized));
                    break;
            }
            return tokens;
        }
//---------------------------------------------------------------------------------------------------------------------------//
    }
}

