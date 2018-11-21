using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace EMRedemption.Services
{
    public class TDescyptoService : ITDesCryptoService
    {
        private const string KEY = "1111111111111111AABBCCDDEEFFAABB1111111111111111";
        public string Encrypt(string ascii)
        {
            byte[] dataBytes = System.Text.Encoding.ASCII.GetBytes(ascii);
            byte[] keyBytes = hexStringToBytes(KEY);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyBytes;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.Zeros;

            ICryptoTransform transform = tdes.CreateEncryptor();
            byte[] resultArray = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

            tdes.Clear();

            return BytesToHexString(resultArray);
        }

        public string Decrypt(string hexStringData)
        {

            byte[] keyBytes = hexStringToBytes(KEY);
            byte[] dataBytes = hexStringToBytes(hexStringData);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyBytes;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.Zeros;

            ICryptoTransform transform = tdes.CreateDecryptor();
            byte[] resultArray = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

            tdes.Clear();

            return System.Text.Encoding.ASCII.GetString(resultArray).TrimEnd('\0');
        }

        private byte[] hexStringToBytes(string stringHex)
        {
            stringHex = stringHex.Replace("-", "");
            byte[] bytes = new byte[stringHex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(stringHex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        private string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
