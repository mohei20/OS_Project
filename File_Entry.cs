using System.Collections.Generic;

namespace OS_Project
{
    public class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
//---------------------------------------------------------------------------------------------------------//
        public File_Entry(string name, byte dir_attr, int dir_first_cluster, Directory pa)
          : base(name, dir_attr, dir_first_cluster)
        {
            this.content = string.Empty;
            if (pa == null)
                return;
            this.parent = pa;
        }
//----------------------------------------------------------------------------------------------------------------------------------------//
        public Directory_Entry Get_Directory() =>new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_first_cluster);
//-------------------------------------------------------------------------------------------------------------------------------------------//
        public void Write_Content()
        {
            List<byte[]> numArrayList = Converter.splitBytes(Converter.StringToBytes(this.content));
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
        }
//--------------------------------------------------------------------------------------------------------------------//
        public void Read_Content()
        {
            if (this.dir_first_cluster == 0)
                return;
            this.content = string.Empty;
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
            this.content = Converter.BytesToString(byteList.ToArray());
        }
//----------------------------------------------------------------------------------------------------//
        public void Delete()
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
            if (this.parent == null)
                return;
            int index = this.parent.Search_for_Directory(new string(this.dir_name));
            if (index != -1)
            {
                this.parent.entires.RemoveAt(index);
                this.parent.Write_Directory();
                Mini_FAT.Write_FAT();
            }
        }
//-----------------------------------------------------------------------------------------------------//
        
    }
}

