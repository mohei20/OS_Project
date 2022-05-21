using System;

namespace OS_Project
{
    [Serializable]
    public class Directory_Entry
    {
//--------------------------------Attributes---------------------------------------------------------//
        public char[] dir_name = new char[11];
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_first_cluster;
        public int dir_file_size;
//--------------------------------Empty Constructor---------------------------------------------------------//
        public Directory_Entry()
        {
        }
//---------------------------------Parametraized Constructor---------------------------------------------//
        public Directory_Entry(string name, byte dir_attr, int dir_first_cluster)
        {
            this.dir_attr = dir_attr;
            switch (dir_attr)
            {
                case 0:
                    string[] strArray = name.Split('.');
                    this.Assign_File_Name(strArray[0].ToCharArray(), strArray[1].ToCharArray());
                    break;
                case 16:
                    this.Assign_Dir_Name(name.ToCharArray());
                    break;
            }
            this.dir_first_cluster = dir_first_cluster;
        }
//------------------------------------------------------------------------------------------------------------//
        public void Assign_File_Name(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int num1 = 0;
                for (int index = 0; index < name.Length; ++index)
                {
                    ++num1;
                    this.dir_name[index] = name[index];
                }
                int index1 = num1 + 1;
                this.dir_name[index1] = '.';
                for (int index2 = 0; index2 < extension.Length; ++index2)
                {
                    ++index1;
                    this.dir_name[index1] = extension[index2];
                }
                int num2;
                for (int index3 = num2 = index1 + 1; index3 < this.dir_name.Length; ++index3)
                    this.dir_name[index3] = ' ';
            }
            else
            {
                for (int index = 0; index < 7; ++index)
                    this.dir_name[index] = name[index];
                this.dir_name[7] = '.';
                int index4 = 0;
                int index5 = 8;
                for (; index4 < extension.Length; ++index4)
                {
                    this.dir_name[index5] = extension[index4];
                    ++index5;
                }
            }
        }
//----------------------------------------------------------------------------------------------------//
        public void Assign_Dir_Name(char[] name)
        {
            if (name.Length <= 11)
            {
                int num1 = 0;
                for (int index = 0; index < name.Length; ++index)
                {
                    ++num1;
                    this.dir_name[index] = name[index];
                }
                int num2;
                for (int index = num2 = num1 + 1; index < this.dir_name.Length; ++index)
                    this.dir_name[index] = ' ';
            }
            else
            {
                int num = 0;
                for (int index = 0; index < 11; ++index)
                {
                    ++num;
                    this.dir_name[index] = name[index];
                }
            }
        }
//----------------------------------------------------------------------------------------------------------//
    }
}

