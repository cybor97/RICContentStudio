using System;
using System.Collections.Generic;

namespace RICContentStudio
{
    public static class StringEncryptor
    {
        public static List<int> Encrypt(string TextToEncrypt, int EncryptingKey)
        {
            List<int> Data = new List<int>();
            foreach (var CurrentChar in TextToEncrypt)
                if (EncryptingKey != 0)
                    Data.Add((int)CurrentChar * EncryptingKey);
                else
                    Data.Add((int)CurrentChar);
            return Data;
        }
        public static string Decrypt(List<int> DataToDecrypt, int DecryptingKey)
        {
            string DecryptedString = string.Empty;
            foreach (var DataBlock in DataToDecrypt)
                if (DecryptingKey != 0) DecryptedString += (char)(DataBlock / DecryptingKey);
                else DecryptedString += (char)DataBlock;
            return DecryptedString;
        }
        public static List<int> GetData(string PackedData)
        {
            List<string> strData = new List<string>();
            List<int> Data = new List<int>();
            bool BeginBlock = false;
            int i = -1;
            foreach (var CurrentChar in PackedData)
            {
                if (CurrentChar == '[') { BeginBlock = true; strData.Add(string.Empty); i++; };
                if (CurrentChar == ']') BeginBlock = false;
                if ((CurrentChar != '[') && (CurrentChar != ']') && BeginBlock)
                    strData[i] += CurrentChar;
            }
            foreach (var CurrentStr in strData)
                Data.Add(Convert.ToInt32(CurrentStr));
            return Data;
        }
        public static string PackData(List<int> InitialData)
        {
            string PackedData = string.Empty;
            foreach (var DataBlock in InitialData)
                PackedData += "[" + DataBlock + "]";
            return PackedData;
        }
        public static int GetKey(string StringWithKey)
        {
            int Key = 0;
            foreach (var CurrentChar in StringWithKey)
                Key += (int)CurrentChar;
            return Key;
        }
    }
}
