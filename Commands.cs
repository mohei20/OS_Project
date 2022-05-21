using System;
using System.Collections.Generic;

namespace OS_Project
{
    public static class Commands
    {
//------------------------------------cls------------------------------------------------------------------------------//
        public static void Clear_Screen(List<Token> tokens)
        {
            if (tokens.Count > 1)
                Console.WriteLine( tokens[0].value + " Command syntax is \n cls \n function: Clear the screen.");
            else
                Console.Clear();
        }
//------------------------------------quit-------------------------------------------------------------------//
        public static void quit(List<Token> tokens)
        {
            if (tokens.Count > 1)
            {
                Console.WriteLine(tokens[0].value + " Command syntax is \n quit \n function: Quit the shell.");
            }
            else
            {
                Mini_FAT.Write_FAT();
                Virtual_Disk.Disk.Close();
                Environment.Exit(0);
            }
        }
//----------------------------------------md-----------------------------------------------------------------------------------------//
        public static void Create_Directory(List<Token> tokens)
        {
            if (tokens.Count <= 1 || tokens.Count > 2)
            {
                Console.WriteLine(tokens[0].value + " Command syntax is \n md [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            }
            else if (tokens[1].key == Type_Of_Token.DirName)
            {
                if (Program.Current_Dir.Search_for_Directory(tokens[1].value) == -1)
                {
                    if (Mini_FAT.Get_Empty_Place() != -1)
                    {
                        Directory_Entry Dir_Entry = new Directory_Entry(tokens[1].value, (byte)16, 0);
                        Program.Current_Dir.entires.Add(Dir_Entry);
                        Program.Current_Dir.Write_Directory();

                        if (Program.Current_Dir.parent != null)
                        {
                            Program.Current_Dir.parent.Update(Program.Current_Dir.Get_Directory());
                            Program.Current_Dir.parent.Write_Directory();
                        }
                        Mini_FAT.Write_FAT();
                        Console.WriteLine("directory \" " + tokens[1].value + " \" has created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Sorry the disk is full!");
                    }
                }
                else
                {
                    Console.WriteLine("this directory \" " + tokens[1].value + " \" is already exists!");
                }
            }
            else if (tokens[1].key == Type_Of_Token.FullPath_To_Directory)
            {
                Directory directory = Commands.Move_To_Dir(tokens[1], false);
                if (directory == null)
                    Console.WriteLine("this path \" " + tokens[1].value + " \" is not exists!");
                else if (Mini_FAT.Get_Empty_Place() != -1)
                {
                    string[] strArray = tokens[1].value.Split('\\');
                    Directory_Entry directoryEntry = new Directory_Entry(strArray[strArray.Length - 1], (byte)16, 0);
                    directory.entires.Add(directoryEntry);
                    directory.Write_Directory();
                    directory.parent.Update(directory.Get_Directory());
                    directory.parent.Write_Directory();
                    Mini_FAT.Write_FAT();
                    Console.WriteLine("directory \" " + tokens[1].value + " \" has created successfully!");
                }
                else
                    Console.WriteLine("Sorry the disk is full!");
            }
            else
                Console.WriteLine(tokens[0].value + " Command syntax is \n md [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
        }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        public static File_Entry Create_File(Token token)
        {
            if (token.key == Type_Of_Token.File_Name)
            {
                if (Program.Current_Dir.Search_for_Directory(token.value) == -1)
                {
                    if (Mini_FAT.Get_Empty_Place() != -1)
                    {
                        Directory_Entry directoryEntry = new Directory_Entry(token.value, (byte)0, 0);
                        Program.Current_Dir.entires.Add(directoryEntry);
                        Program.Current_Dir.Write_Directory();
                        if (Program.Current_Dir.parent != null)
                        {
                            Program.Current_Dir.parent.Update(Program.Current_Dir.Get_Directory());
                            Program.Current_Dir.parent.Write_Directory();
                        }
                        Mini_FAT.Write_FAT();
                        return new File_Entry(token.value, (byte)0, 0, Program.Current_Dir);
                    }
                    Console.WriteLine("Sorry the disk is full!");
                }
                else
                    Console.WriteLine("this file name \" " + token.value + " \" is already exists!");
            }
            else if (token.key == Type_Of_Token.FullPath_To_File)
            {
                Directory pa = Commands.Move_To_Dir(new Token()
                {
                    value = token.value.Substring(0, token.value.LastIndexOf('\\')),
                    key = token.key
                }, false);
                if (pa == null)
                {
                    Console.WriteLine("this path \" " + token.value + " \" is not exists!");
                }
                else
                {
                    if (Mini_FAT.Get_Empty_Place() != -1)
                    {
                        string[] strArray = token.value.Split('\\');
                        Directory_Entry directoryEntry = new Directory_Entry(strArray[strArray.Length - 1], (byte)16, 0);
                        pa.entires.Add(directoryEntry);
                        pa.Write_Directory();
                        pa.parent.Update(pa.Get_Directory());
                        pa.parent.Write_Directory();
                        Mini_FAT.Write_FAT();
                        return new File_Entry(strArray[strArray.Length - 1], (byte)0, 0, pa);
                    }
                    Console.WriteLine("Sorry the disk is full!");
                }
            }
            return (File_Entry)null;
        }
//------------------------------------------------------------------------------------------------------------------------------//
        private static Directory Move_To_Dir(Token token, bool changedirFlag)
        {
            Directory directory1 = (Directory)null;
            if (token.key == Type_Of_Token.DirName)
            {
                if (token.value != "..")
                {
                    int index = Program.Current_Dir.Search_for_Directory(token.value);
                    if (index == -1)
                        return (Directory)null;
                    string name = new string(Program.Current_Dir.entires[index].dir_name);
                    byte dirAttr = Program.Current_Dir.entires[index].dir_attr;
                    int dirFirstCluster = Program.Current_Dir.entires[index].dir_first_cluster;
                    directory1 = new Directory(name, dirAttr, dirFirstCluster, Program.Current_Dir);
                    directory1.Read_Directory();
                    string str = Program.Current_Dir_Path + "\\" + name.Trim(char.MinValue, ' ');
                    if (changedirFlag)
                        Program.Current_Dir_Path = str;
                }
                else if (Program.Current_Dir.parent != null)
                {
                    directory1 = Program.Current_Dir.parent;
                    directory1.Read_Directory();
                    string Current_Dir_Path = Program.Current_Dir_Path;
                    string str = Current_Dir_Path.Substring(0, Current_Dir_Path.LastIndexOf('\\'));
                    if (changedirFlag)
                        Program.Current_Dir_Path = str;
                }
                else
                {
                    directory1 = Program.Current_Dir;
                    directory1.Read_Directory();
                }
            }
            else if (token.key == Type_Of_Token.FullPath_To_Directory)
            {
                string[] strArray = token.value.Split('\\');
                List<string> stringList = new List<string>();
                for (int index = 0; index < strArray.Length; ++index)
                {
                    if (strArray[index] != "")
                        stringList.Add(strArray[index]);
                }
                Directory directory2 = new Directory("m:", (byte)16, 5, (Directory)null);
                directory2.Read_Directory();
                if (stringList[0].ToLower().Equals("m:"))
                {
                    string str = "m:";
                    int num = changedirFlag ? stringList.Count : stringList.Count - 1;
                    for (int index1 = 1; index1 < num; ++index1)
                    {
                        int index2 = directory2.Search_for_Directory(stringList[index1]);
                        if (index2 == -1)
                            return (Directory)null;
                        Directory pa = directory2;
                        string name = new string(directory2.entires[index2].dir_name);
                        byte dirAttr = directory2.entires[index2].dir_attr;
                        int dirFirstCluster = directory2.entires[index2].dir_first_cluster;
                        directory2 = new Directory(name, dirAttr, dirFirstCluster, pa);
                        directory2.Read_Directory();
                        str = str + "\\" + name.Trim(char.MinValue, ' ');
                    }
                    directory1 = directory2;
                    if (changedirFlag)
                        Program.Current_Dir_Path = str;
                } 
            }
            return directory1;
        }
//----------------------------------------------cd--------------------------------------------------------------//
        public static void Change_Directory(List<Token> tokens)
        {
            if (tokens.Count == 1)
                Console.WriteLine(Program.Current_Dir_Path);
            else if (tokens.Count == 2)
            {
                if (tokens[1].key == Type_Of_Token.DirName || tokens[1].key == Type_Of_Token.FullPath_To_Directory)
                {
                    Directory directory = Commands.Move_To_Dir(tokens[1], true);
                    if (directory != null)
                    {
                        directory.Read_Directory();
                        Program.Current_Dir = directory;
                    }
                    else
                        Console.WriteLine("this path \" " + tokens[1].value + " \" is not exists!");
                }
                else
                    Console.WriteLine(tokens[0].value + " Command syntax is \n cd \n or \n cd [directory]\n[directory] can be a new directory name or fullpath of a directory");
            }
            else
                Console.WriteLine(tokens[0].value + "Command syntax is \n cd \n or \n cd [directory]\n[directory] can be a new directory name or fullpath of a directory");
        }
//--------------------------------------------help----------------------------------------------------------------------------------------------------------------------------------//
        public static void help(List<Token> tokens)
        {
            if (tokens.Count > 2)
            {
                Console.WriteLine("Provides help information for Shell commands.\n" + tokens[0].value + "[command]\n command - displays help information on that command.");
            }
            else if (tokens.Count == 2)
            {
                if (tokens[1].key == Type_Of_Token.Command)
                {
                    string Command = tokens[1].value;

                    if (Command == "cls")
                    {
                        Console.WriteLine("Clear the screen.");
                        Console.WriteLine("CLS");

                    }
                    else if (Command == "help")
                    {
                        Console.WriteLine("Provides help information for Shell commands.\n" + tokens[0].value + "[command]\n command - displays help information on that command.");
                    }
                    else if (Command == "import")
                    {
                        Console.WriteLine("– import text file(s) from your computer ");
                        Console.WriteLine(tokens[1].value + " command syntax is \n import [destination] [file]+");
                        Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                        Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                        Console.WriteLine("[destination] can be directory name or fullpath of a directory in your implemented file system");
                    }

                    else if (Command == "quit")
                    {
                        Console.WriteLine("Quit the shell.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n quit");
                    }
                    else if (Command == "type")
                    {
                        Console.WriteLine("Displays the contents of a text file.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n type [file]");
                        Console.WriteLine("NOTE: it displays the filename before its content for every file");
                        Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                    }
                    else if (Command == "rd")
                    {
                        Console.WriteLine("Removes a directory.");
                        Console.WriteLine("NOTE: it confirms the user choice to delete the directory before deleting");
                        Console.WriteLine(tokens[1].value + " command syntax is \n rd [directory]");
                        Console.WriteLine("[directory] can be a directory name or fullpath of a directory");
                    }
                    else if (Command == "cd")
                    {
                        Console.WriteLine("Change the Current_Dir default directory to the directory given in the argument.");
                        Console.WriteLine("If the argument is not present, report the Current_Dir directory.");
                        Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n cd \n or \n cd [directory]");
                        Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                    }
                    else if (Command == "md")
                    {
                        Console.WriteLine("Creates a directory.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n md [directory]");
                        Console.WriteLine("[directory] can be a new directory name or fullpath of a new directory");
                    }
                    else if (Command == "rename")
                    {
                        Console.WriteLine("Renames a file.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n rd [fileName] [new fileName]");
                        Console.WriteLine("[fileName] can be a file name or fullpath of a filename ");
                        Console.WriteLine("[new fileName] can be a new file name not fullpath ");
                    }
                    else if (Command == "del")
                    {
                        Console.WriteLine("Deletes one or more files.");
                        Console.WriteLine("NOTE: it confirms the user choice to delete the file before deleting");
                        Console.WriteLine(tokens[1].value + " command syntax is \n del [file]+");
                        Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file)");
                        Console.WriteLine("[file] can be file Name (or fullpath of file)");
                    }
                    else if (Command == "copy")
                    {
                        Console.WriteLine("Copies one or more files to another location.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n copy [source]+ [destination]");
                        Console.WriteLine("+ after [source] represent that you can pass more than file Name (or fullpath of file) or more than directory Name (or fullpath of directory)");
                        Console.WriteLine("[source] can be file Name (or fullpath of file) or directory Name (or fullpath of directory)");
                        Console.WriteLine("[destination] can be directory name or fullpath of a directory");
                    }
                    else if (Command == "dir")
                    {
                        Console.WriteLine("List the contents of directory given in the argument.");
                        Console.WriteLine("If the argument is not present, list the content of the Current_Dir directory.");
                        Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                        Console.WriteLine(tokens[1].value + " command syntax is \n dir \n or \n dir [directory]");
                        Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                    }
                    else if (Command == "export")
                    {
                        Console.WriteLine("– export text file(s) to your computer ");
                        Console.WriteLine(tokens[1].value + " command syntax is \n export [destination] [file]+");
                        Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                        Console.WriteLine("[file] can be file Name (or fullpath of file) of text file in your implemented file system");
                        Console.WriteLine("[destination] can be directory name or fullpath of a directory in your computer");
                    }
                }
                else
                    Console.WriteLine("This command is not supported by the help utility.");
            }
            else
            {
                if (tokens.Count != 1)
                    return;
                Console.WriteLine("cd       - Change the Current_Dir default directory to .");
                Console.WriteLine("           If the argument is not present, report the Current_Dir directory.");
                Console.WriteLine("           If the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls      - Clear the screen.");
                Console.WriteLine("dir      - List the contents of directory .");
                Console.WriteLine("quit     - Quit the shell.");
                Console.WriteLine("copy     - Copies one or more files to another location");
                Console.WriteLine("del      - Deletes one or more files.");
                Console.WriteLine("help     - Provides Help information for commands.");
                Console.WriteLine("md       - Creates a directory.");
                Console.WriteLine("rd       - Removes a directory.");
                Console.WriteLine("rename   - Renames a file.");
                Console.WriteLine("type     - Displays the contents of a text file.");
                Console.WriteLine("import   – import text file(s) from your computer");
                Console.WriteLine("export   – export text file(s) to your computer");
            }
        }
//--------------------------------------dir-------------------------------------------------//
        public static void List_Directory(List<Token> tokens)
        {
            if (tokens.Count == 1)
            {
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                Console.WriteLine(" Directory of " + Program.Current_Dir_Path);
                Console.WriteLine();
                int num4 = 1;
                if (Program.Current_Dir.parent != null)
                {
                    num4 = 2;
                    Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)".");
                    int num5 = num2 + 1;
                    Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)"..");
                    num2 = num5 + 1;
                }
                for (int index = num4; index < Program.Current_Dir.entires.Count; ++index)
                {
                    if (Program.Current_Dir.entires[index].dir_attr == (byte)0)
                    {
                        Console.WriteLine("\t{0:9}{1:11}", (object)Program.Current_Dir.entires[index].dir_file_size, (object)new string(Program.Current_Dir.entires[index].dir_name));
                        ++num1;
                        num3 += Program.Current_Dir.entires[index].dir_file_size;
                    }
                    else if (Program.Current_Dir.entires[index].dir_attr == (byte)16)
                    {
                        Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)new string(Program.Current_Dir.entires[index].dir_name));
                        ++num2;
                    }
                }
                Console.WriteLine(string.Format("{0}{1} File(s)    {2} bytes", (object)"              ", (object)num1, (object)num3));
                Console.WriteLine(string.Format("{0}{1} Dir(s)    {2} bytes free", (object)"              ", (object)num2, (object)Virtual_Disk.Get_Free_Space()));

            }
            else if (tokens.Count == 2)
            {
                if (tokens[1].key == Type_Of_Token.DirName || tokens[1].key == Type_Of_Token.FullPath_To_Directory)
                {
                    Directory directory = Commands.Move_To_Dir(tokens[1], false);
                    if (directory != null)
                    {
                        directory.Read_Directory();
                        int num6 = 0;
                        int num7 = 0;
                        int num8 = 0;
                        if (tokens[1].key == Type_Of_Token.DirName)
                            Console.WriteLine(" Directory of " + Program.Current_Dir_Path + "\\" + tokens[1].value);
                        else
                            Console.WriteLine(" Directory of " + tokens[1].value);
                        Console.WriteLine();
                        int num9 = 1;
                        if (directory.parent != null)
                        {
                            num9 = 2;
                            Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)".");
                            int num10 = num7 + 1;
                            Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)"..");
                            num7 = num10 + 1;
                        }
                        for (int index = num9; index < directory.entires.Count; ++index)
                        {
                            if (directory.entires[index].dir_attr == (byte)0)
                            {
                                Console.WriteLine("\t{0:9} {1:11}", (object)directory.entires[index].dir_file_size, (object)new string(directory.entires[index].dir_name));
                                ++num6;
                                num8 += directory.entires[index].dir_file_size;
                            }
                            else if (directory.entires[index].dir_attr == (byte)16)
                            {
                                Console.WriteLine("{0}{1:11}", (object)"\t<DIR>    ", (object)new string(directory.entires[index].dir_name));
                                ++num7;
                            }
                        }
                        Console.WriteLine(string.Format("{0}{1} File(s)    {2} bytes", (object)"              ", (object)num6, (object)num8));
                        Console.WriteLine(string.Format("{0}{1} Dir(s)    {2} bytes free", (object)"              ", (object)num7, (object)Virtual_Disk.Get_Free_Space()));
                    }
                    else
                        Console.WriteLine("the path \" " + tokens[1].value + " \" is not found");
                }
                else
                    Console.WriteLine(tokens[0].value + " Command syntax is \n dir \n or \n dir [directory]\n[directory] can be a new directory name or fullpath of a directory");
            }
            else
                Console.WriteLine(tokens[0].value + " Command syntax is \n dir \n or \n dir [directory]\n[directory] can be a new directory name or fullpath of a directory");
        }
//-----------------------------------------------rd------------------------------------------------------------------------------------------------------------------------------------------//
        public static void Remove_Directory(List<Token> tokens)
        {
            if (tokens.Count <= 1 || tokens.Count > 2)
                Console.WriteLine(tokens[0].value + "Command syntax is \n rd [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            else if (tokens[1].key == Type_Of_Token.DirName || tokens[1].key == Type_Of_Token.FullPath_To_Directory)
            {
                Directory directory = Commands.Move_To_Dir(tokens[1], false);
                if (directory != null)
                {
                    Console.WriteLine("this directory is not empty");
                    Console.Write("Are you sure that you want to delete " + new string(directory.dir_name).Trim(char.MinValue, ' ') + " ? please enter Y for yes or N for no:");
                    if (Console.ReadLine().ToLower().Equals("y"))
                    {
                        directory.Delete_Directory();
                        Console.Write("directory " + new string(directory.dir_name).Trim(char.MinValue, ' ') + " has deleted successfully\n");
                    }
                    else
                    {
                        Console.Write("directory " + new string(directory.dir_name).Trim(char.MinValue, ' ') + " has NOT deleted successfully\n");
                    }
                        
                }
                else
                    Console.WriteLine("directory \" " + tokens[1].value + " \" is not found!");
            }
            else
                Console.WriteLine(tokens[0].value + " Command syntax is \n rd [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
        }
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        
    }
}

