using System.Collections.Generic;

namespace OS_Project
{
    public class Directory : Directory_Entry
    {
        public List<Directory_Entry> entires;
        public Directory parent;
//--------------------------------------------Constructor---------------------------------------------//
        public Directory(string name, byte dir_attr, int dir_firstCluster, Directory parent)
          : base(name, dir_attr, dir_firstCluster)
        {
            this.entires = new List<Directory_Entry>();
            if (parent == null)
                return;
            this.parent = parent;
        }
//-------------------------------------------------------------------------------------------------------------//
        public void Update(Directory_Entry dir)
        {
            int index = this.Search_for_Directory(new string(dir.dir_name));
            if (index == -1)
                return;
            this.entires.RemoveAt(index);
            this.entires.Insert(index, dir);
        }
//------------------------------------------------------------------------------------------------------------//
        public Directory_Entry Get_Directory()
        {
            return (new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_first_cluster));
        }
//--------------------------------------------------------------------------------------------------------------//
        public void Write_Directory()
        {
            byte[] bytes1 = new byte[this.entires.Count * 32];
            for (int index1 = 0; index1 < this.entires.Count; ++index1)
            {
                byte[] bytes2 = Converter.Directory_EntryToBytes(this.entires[index1]);
                int index2 = index1 * 32;
                int index3 = 0;
                while (index3 < bytes2.Length)
                {
                    bytes1[index2] = bytes2[index3];
                    ++index3;
                    ++index2;
                }
            }
            List<byte[]> numArrayList = Converter.splitBytes(bytes1);
            int num;
            if (this.dir_first_cluster != 0)
            {
                num = this.dir_first_cluster;
            }
            else
            {
                num = Mini_FAT.Get_Empty_Place();
                this.dir_first_cluster = num;
            }
            int clusterIndex = -1;
            for (int index = 0; index < numArrayList.Count; ++index)
            {
                if (num != -1)
                {
                    Virtual_Disk.Write_Cluster(numArrayList[index], num, count: numArrayList[index].Length);
                    Mini_FAT.Set_Cluster_Pointer(num, -1);
                    if (clusterIndex != -1)
                        Mini_FAT.Set_Cluster_Pointer(clusterIndex, num);
                    clusterIndex = num;
                    num = Mini_FAT.Get_Empty_Place();
                }
            }
            if (this.parent != null)
            {
                this.parent.Update(this.Get_Directory());
                this.parent.Write_Directory();
            }
            Mini_FAT.Write_FAT();
        }
//--------------------------------------------------------------------------------------------------------------------------//
        public void Read_Directory()
        {
            if (this.dir_first_cluster == 0)
                return;
            this.entires = new List<Directory_Entry>();
            int clusterIndex = this.dir_first_cluster;
            int clusterPointer = Mini_FAT.Get_Cluster_Pointer(clusterIndex);
            List<byte> byteList = new List<byte>();
            do
            {
                byteList.AddRange((IEnumerable<byte>)Virtual_Disk.Read_Cluster(clusterIndex));
                clusterIndex = clusterPointer;
                if (clusterIndex != -1)
                    clusterPointer = Mini_FAT.Get_Cluster_Pointer(clusterIndex);
            }
            while (clusterPointer != -1);
            for (int index1 = 0; index1 < byteList.Count; ++index1)
            {
                byte[] bytes = new byte[32];
                int index2 = index1 * 32;
                for (int index3 = 0; index3 < bytes.Length && index2 < byteList.Count; ++index2)
                {
                    bytes[index3] = byteList[index2];
                    ++index3;
                }
                if (bytes[0] != (byte)0)
                    this.entires.Add(Converter.BytesToDirectory_Entry(bytes));
                else
                    break;
            }
        }
//---------------------------------------------------------------------------------------------------------------//
        public void Delete_Directory()
        {
            if (this.dir_first_cluster != 0)
            {
                int clusterIndex = this.dir_first_cluster;
                int clusterPointer = Mini_FAT.Get_Cluster_Pointer(clusterIndex);
                do
                {
                    Mini_FAT.Set_Cluster_Pointer(clusterIndex, 0);
                    clusterIndex = clusterPointer;
                    if (clusterIndex != -1)
                        clusterPointer = Mini_FAT.Get_Cluster_Pointer(clusterIndex);
                }
                while (clusterIndex != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.Search_for_Directory(new string(this.dir_name));
                if (index != -1)
                {
                    this.parent.entires.RemoveAt(index);
                    this.parent.Write_Directory();
                }
            }
            if (Program.Current_Dir == this && this.parent != null)
            {
                Program.Current_Dir = this.parent;
                Program.Current_Dir_Path = Program.Current_Dir_Path.Substring(0, Program.Current_Dir_Path.LastIndexOf('\\'));
                Program.Current_Dir.Read_Directory();
            }
            Mini_FAT.Write_FAT();
        }
//----------------------------------------------------------------------------------------------------------------------------------------//
        public int Search_for_Directory(string name)
        {
            if (name.Length < 11)
            {
                name += "\0";
                for (int index = name.Length + 1; index < 12; ++index)
                    name += " ";
            }
            else
                name = name.Substring(0, 11);
            for (int index = 0; index < this.entires.Count; ++index)
            {
                if (new string(this.entires[index].dir_name).Equals(name))
                    return index;
            }
            return -1;
        }
    }
//-------------------------------------------------------------------------------------------------------------------------------------------------//
}
