using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Sgms.Frame.Sys;

namespace Sgms.Frame.Utils
{
    /// <summary>
    /// 加密相关
    /// </summary>
    public static class EncryptUtil
    {

        private static string _TmpKey;
        private static string Key
        {
            get
            {
                if (_TmpKey == null)
                {
                    SetKeyIV(SysPara.GUIDKey);
                }
                return _TmpKey;
            }
        }

        private static string _TmpIV;
        private static string IV
        {
            get
            {
                if (_TmpIV == null)
                {
                    SetKeyIV(SysPara.GUIDKey);
                }
                return _TmpIV;
            }
        }

        /// <summary>
        /// 设置 key和iv
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public static void SetKeyIV(string key, string iv)
        {
            _TmpKey = key;
            _TmpIV = iv;
        }

        /// <summary>
        /// 截取guid 0-7位作为Key 8-15位作为IV 
        /// </summary>
        /// <param name="guid"></param>
        public static void SetKeyIV(string guid)
        {
            guid = guid.Replace("-", "");
            string[] result = new string[2];
            _TmpKey = guid.Substring(0, 8);
            _TmpIV = guid.Substring(8, 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPwdType"></param>
        /// <param name="strPwd"></param>
        public static string PwdCalculate(string strPwdType, string strPwd)
        { 
            MD5 md1 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md1.ComputeHash(new UnicodeEncoding().GetBytes(strPwd)));
        }

        /// <summary>
        /// Des减密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Des加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5(String text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Md5(bytes);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Md5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha1(string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }


        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="encryptType">可选值MD5 sha1</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetSign(string encryptType, params object[] args)
        {
            string[] arr = new string[args.Length / 2];
            for (var i = 0; i < args.Length; i += 2)
            {
                arr[i / 2] = new StringBuilder().Append(args[i]).Append('=').Append(args[i + 1]).ToString();
            }
            Array.Sort(arr);
            var queryStr = String.Join("&", arr);
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(queryStr, encryptType);
        }
    }
}
