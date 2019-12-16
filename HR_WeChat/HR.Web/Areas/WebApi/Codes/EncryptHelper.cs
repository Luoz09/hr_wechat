using System;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace HR.Web.Areas.WebApi.Codes
{
    public class EncryptHelper
    {
        private static readonly string Key = GetKey()[0];
        private static readonly string IV = GetKey()[1];

        public static string[] GetKey()
        {
            string guid = ConfigurationManager.AppSettings["Key"];
            if (guid == null || guid.Length < 16) guid = Guid.NewGuid().ToString();
            guid = guid.Replace("-", "");
            string[] result = new string[2];
            result[0] = guid.Substring(0, 8);
            result[1] = guid.Substring(8, 8);
            return result;
        }

        public static string DesDecrypt(string str)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.IV = Encoding.UTF8.GetBytes(IV);
            des.Key = Encoding.UTF8.GetBytes(Key);
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0; i < str.Length; i = i + 2)
            {
                buffer[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            }
            buffer = des.CreateDecryptor(des.Key, des.IV).TransformFinalBlock(buffer, 0, buffer.Length);
            des.Clear();
            return Encoding.UTF8.GetString(buffer);
        }

        //这是加密
        public static string DesEncrypt(string str)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.IV = Encoding.UTF8.GetBytes(IV);
            des.Key = Encoding.UTF8.GetBytes(Key);
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            buffer = des.CreateEncryptor(des.Key, des.IV).TransformFinalBlock(buffer, 0, buffer.Length);
            des.Clear();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string Md5(String text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }
    }
}
