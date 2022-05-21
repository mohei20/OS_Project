using System;
using System.Collections.Generic;

namespace OS_Project
{
    public static class Mini_FAT
    {
        public static int[] FAT = new int[1024];

        public static void Prepare_FAT()
        {
            for (int index = 0; index < Mini_FAT.FAT.Length; ++index)
            {
                if (index == 0 || index == 4)
                {
                    Mini_FAT.FAT[index] = -1;
                }
                else
                {
                    int num = index <= 0 ? 0 : (index <= 3 ? 1 : 0);
                    Mini_FAT.FAT[index] = num == 0 ? 0 : index + 1;
                }
            }
        }
//-------------------------------------------------------------------------------------------//
        public static void Write_FAT()
        {
            List<byte[]> numArrayList = Converter.splitBytes(Converter.ToBytes(Mini_FAT.FAT));
            for (int index = 0; index < numArrayList.Count; ++index)
                Virtual_Disk.Write_Cluster(numArrayList[index], index + 1, count: numArrayList[index].Length);
        }
//-------------------------------------------------------------------------------------------//
        public static void Read_FAT()
        {
            List<byte> byteList = new List<byte>();
            for (int clusterIndex = 1; clusterIndex <= 4; ++clusterIndex)
                byteList.AddRange((IEnumerable<byte>)Virtual_Disk.Read_Cluster(clusterIndex));
            Mini_FAT.FAT = Converter.ToInt(byteList.ToArray());
        }
//---------------------------------------------------------------------------------------------//
        public static void Print_FAT()
        {
            Console.WriteLine("FAT has the following: ");
            for (int index = 0; index < Mini_FAT.FAT.Length; ++index)
                Console.WriteLine("FAT[" + index.ToString() + "] = " + Mini_FAT.FAT[index].ToString());
        }
//------------------------------------------------------------------------------------------------//
        public static void Set_FAT(int[] arr)
        {
            if (arr.Length > 1024)
                return;
            Mini_FAT.FAT = arr;
        }
//---------------------------------------------------------------------------------------------------------------//
        public static int Get_Empty_Place()
        {
            for (int avilableCluster = 0; avilableCluster < Mini_FAT.FAT.Length; ++avilableCluster)
            {
                if (Mini_FAT.FAT[avilableCluster] == 0)
                    return avilableCluster;
            }
            return -1;
        }
//----------------------------------------------------------------------------------------------------------------//
        public static void Set_Cluster_Pointer(int Cluster_Index, int value)
        {
            FAT[Cluster_Index] = value;

        }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------//
        public static int Get_Cluster_Pointer(int clusterIndex) => clusterIndex >= 0 && clusterIndex < Mini_FAT.FAT.Length ? Mini_FAT.FAT[clusterIndex] : -1;
//-------------------------------------------------------------------------------------------------------------------------------------------------------------//
    }
}

