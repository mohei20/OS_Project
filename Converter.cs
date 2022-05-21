using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace OS_Project
{
    public static class Converter
    {
//--------------------------------------------------------------------------------------------------//
        public static byte[] ToBytes(int[] array)
        {
            byte[] dest = new byte[array.Length * 4];
            Buffer.BlockCopy((Array)array, 0, (Array)dest, 0, dest.Length);
            return dest;
        }
//-----------------------------------------------------------------------------------------------------//
        public static byte[] StringToBytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int index = 0; index < s.Length; ++index)
                bytes[index] = (byte)s[index];
            return bytes;
        }
//---------------------------------------------------------------------------------------------------------//
        public static string BytesToString(byte[] bytes)
        {
            string empty_string = string.Empty;
            for (int index = 0; index < bytes.Length && bytes[index] > (byte)0; ++index)
                empty_string += ((char)bytes[index]).ToString();
            return empty_string;
        }
//--------------------------------------------------------------------------------------------------------------//
        public static byte[] Directory_EntryToBytes(Directory_Entry d)
        {
            byte[] bytes = new byte[32];
            for (int index = 0; index < d.dir_name.Length; ++index)
                bytes[index] = (byte)d.dir_name[index];
            bytes[11] = d.dir_attr;
            int index1 = 12;
            for (int index2 = 0; index2 < d.dir_empty.Length; ++index2)
            {
                bytes[index1] = d.dir_empty[index2];
                ++index1;
            }
            foreach (byte num in BitConverter.GetBytes(d.dir_first_cluster))
            {
                bytes[index1] = num;
                ++index1;
            }
            foreach (byte num in BitConverter.GetBytes(d.dir_file_size))
            {
                bytes[index1] = num;
                ++index1;
            }
            return bytes;
        }
//---------------------------------------------------------------------------------------------------------//
        public static Directory_Entry BytesToDirectory_Entry(byte[] bytes)
        {
            char[] chArray = new char[11];
            for (int index = 0; index < chArray.Length; ++index)
                chArray[index] = (char)bytes[index];
            byte dir_attr = bytes[11];
            byte[] numArray1 = new byte[12];
            int index1 = 12;
            for (int index2 = 0; index2 < numArray1.Length; ++index2)
            {
                numArray1[index2] = bytes[index1];
                ++index1;
            }
            byte[] numArray2 = new byte[4];
            for (int index3 = 0; index3 < numArray2.Length; ++index3)
            {
                numArray2[index3] = bytes[index1];
                ++index1;
            }
            int int32_1 = BitConverter.ToInt32(numArray2, 0);
            byte[] numArray3 = new byte[4];
            for (int index4 = 0; index4 < numArray3.Length; ++index4)
            {
                numArray3[index4] = bytes[index1];
                ++index1;
            }
            int int32_2 = BitConverter.ToInt32(numArray3, 0);
            return new Directory_Entry(new string(chArray), dir_attr, int32_1)
            {
                dir_empty = numArray1,
                dir_file_size = int32_2
            };
        }
//-----------------------------------------------------------------------------------------------------//
        public static byte[] ToBytes(List<Directory_Entry> array)
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize((Stream)serializationStream, (object)array);
                return serializationStream.ToArray();
            }
        }
 //-----------------------------------------------------------------------------------------------------------//
        public static int[] ToInt(byte[] bytes)
        {
            int[] dst = new int[bytes.Length / 4];
            Buffer.BlockCopy((Array)bytes, 0, (Array)dst, 0, bytes.Length);
            return dst;
        }
//-----------------------------------------------------------------------------------------------------------------//
        public static List<Directory_Entry> ToDirectory_Entry(byte[] bytes)
        {
            using (MemoryStream serializationStream = new MemoryStream(bytes))
            {
                serializationStream.Seek(0L, SeekOrigin.Begin);
                return ((IEnumerable)new BinaryFormatter().Deserialize((Stream)serializationStream)).Cast<Directory_Entry>().ToList<Directory_Entry>();
            }
        }
//--------------------------------------------------------------------------------------------------------------------------//
        public static List<byte[]> splitBytes(byte[] bytes)
        {
            List<byte[]> numArrayList = new List<byte[]>();
            int num1 = bytes.Length / 1024;
            int num2 = bytes.Length % 1024;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                byte[] numArray = new byte[1024];
                int index2 = index1 * 1024;
                for (int index3 = 0; index3 < 1024; ++index3)
                {
                    numArray[index3] = bytes[index2];
                    ++index2;
                }
                numArrayList.Add(numArray);
            }
            if (num2 > 0)
            {
                byte[] numArray = new byte[1024];
                int index4 = num1 * 1024;
                for (int index5 = 0; index5 < num2; ++index5)
                {
                    numArray[index5] = bytes[index4];
                    ++index4;
                }
                numArrayList.Add(numArray);
            }
            return numArrayList;
        }
//--------------------------------------------------------------------------------------------------------//
    }
}

