using System;
using System.IO;

namespace OS_Project
{
    public static class Virtual_Disk
    {
        public static FileStream Disk;
//-------------------------------------------------------------------------------------------------------------------------------------------------------//
        public static void Create_or_Open_Disk(string path) => Virtual_Disk.Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//-------------------------------------------------------------------------------------------------------------------------------------------------------//
        public static int Get_Free_Space() => 1048576 - (int)Virtual_Disk.Disk.Length;
//-------------------------------------------------------------------------------------------------------------------------------------------------------//
        public static void Initialize(string path)
        {

            if (!File.Exists(path))
            {
                Virtual_Disk.Create_or_Open_Disk(path);
                byte[] cluster = new byte[1024];
                for (int index = 0; index < cluster.Length; ++index)
                    cluster[index] = (byte)0;
                Virtual_Disk.Write_Cluster(cluster, 0);
                Mini_FAT.Prepare_FAT();
                Directory directory = new Directory("M:", (byte)16, 5, (Directory)null);
                directory.Write_Directory();
                Mini_FAT.Set_Cluster_Pointer(5, -1);
                Program.Current_Dir = directory;
                Mini_FAT.Write_FAT();
            }
            else
            {
                Virtual_Disk.Create_or_Open_Disk(path);
                Mini_FAT.Read_FAT();
                Directory directory = new Directory("M:", (byte)16, 5, (Directory)null);
                directory.Read_Directory();
                Program.Current_Dir = directory;
            }

        }
//---------------------------------------------------------------------------------------------------------------------------//
        public static void Write_Cluster(byte[] cluster, int clusterIndex, int offset = 0, int count = 1024)
        {
            Virtual_Disk.Disk.Seek((long)(clusterIndex * 1024), SeekOrigin.Begin);
            Virtual_Disk.Disk.Write(cluster, offset, count);
            Virtual_Disk.Disk.Flush();
        }
//--------------------------------------------------------------------------------------------------------------------------//
        public static byte[] Read_Cluster(int clusterIndex)
        {
            Virtual_Disk.Disk.Seek((long)(clusterIndex * 1024), SeekOrigin.Begin);
            byte[] buffer = new byte[1024];
            Virtual_Disk.Disk.Read(buffer, 0, 1024);
            return buffer;
        }
//--------------------------------------------------------------------------------------------------------------------------//
    }
}

