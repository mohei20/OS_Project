using System;

namespace OS_Project
{
    internal class Program
    {
        public static Directory Current_Dir;
        public static string Current_Dir_Path;
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to our Virtual Disk Shell\n");
            Console.WriteLine("Developed By Mohei & Mokhtar & Saied\n");
            Console.WriteLine("(c) MMS Corporation. All rights reserved.\n");
            Virtual_Disk.Initialize("Disk"); //Initialize Virtual Disk his name is Disk
            //Mini_FAT.Print_FAT();  //Print fat table
            Program.Current_Dir_Path = new string(Program.Current_Dir.dir_name);
            Program.Current_Dir_Path = Program.Current_Dir_Path.Trim(char.MinValue, ' ');
            while (true)
            {
                Console.Write(Program.Current_Dir_Path + "\\>");
                Program.Current_Dir.Read_Directory();
                Parser.parse(Console.ReadLine());
            }
        }
    }
}
