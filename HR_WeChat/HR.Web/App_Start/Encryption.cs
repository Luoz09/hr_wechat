using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HR.Web
{
    public class Encryption
    {
        /// <summary>
        /// 基础MD5码转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TransformationMD5(string data)
        {
            MD5CryptoServiceProvider MD5Data = new MD5CryptoServiceProvider();
            return BitConverter.ToString(MD5Data.ComputeHash(Encoding.Default.GetBytes(data))).Replace("-", "");
        }
        public static string PwdCalculate(string strPwdType, string strPwd)
        {
            string text1 = strPwd;
            MD5 md1 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md1.ComputeHash(new UnicodeEncoding().GetBytes(strPwd)));
        }
        /// <summary>
        /// 基础DES加密
        /// </summary>
        /// <param name="pToEncrypt">待加密数据</param>
        /// <param name="sKey">密匙</param>
        /// <param name="sIV">偏移量</param>
        /// <returns>密匙、偏移量只能为8位</returns>
        public static string DESEncrypt(string pToEncrypt, string sKey, string sIV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider(); //把字符串放到byte数组中

            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            if (sKey.Length < 8)
                sKey = sKey.PadRight(8, '#');
            else
                sKey = sKey.Substring(0, 8);
            if (sIV.Length < 8)
                sIV = sIV.PadRight(8, '#');
            else
                sIV = sIV.Substring(0, 8);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量
            des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);  //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            MemoryStream ms = new MemoryStream();   //使得输入密码必须输入英文文本
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        /// <summary>
        /// 基础DES解密
        /// </summary>
        /// <param name="pToEncrypt">待加密数据</param>
        /// <param name="sKey">密匙</param>
        /// <param name="sIV">偏移量</param>
        /// <returns>密匙、偏移量只能为8位</returns>
        public static string DesDecrypt(string pToDecrypt, string sKey, string sIV)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                //if (sKey.Length < 8 || sKey.Length > 8)
                //    return "密匙只能为8位长度";
                //if (sIV.Length < 8 || sIV.Length > 8)
                //    return "偏移量只能为8位长度";
                if (sKey.Length < 8)
                    sKey = sKey.PadRight(8, '#');
                else
                    sKey = sKey.Substring(0, 8);
                if (sIV.Length < 8)
                    sIV = sIV.PadRight(8, '#');
                else
                    sIV = sIV.Substring(0, 8);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();

            }
            catch (Exception ex)
            {

            }

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// 基础AES加密
        /// </summary>
        /// <param name="toEncrypt">待加密数据</param>
        /// <param name="sKey">密匙</param>
        /// <returns></returns>
        public static string AESEncrypt(string toEncrypt, string sKey)
        {
            //判断密匙长度，如低于32位则补满，高于32则区前32位
            if (sKey.Length < 32)
                sKey = sKey.PadRight(32, '#');
            else
                sKey = sKey.Substring(0, 32);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(sKey);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// 基础AES解密
        /// </summary>
        /// <param name="toDecrypt">待解密数据</param>
        /// <param name="sKey">密匙</param>
        /// <returns></returns>
        public static string AESDecrypt(string toDecrypt, string sKey)
        {
            //判断密匙长度，如低于32位则补满，高于32则区前32位
            if (sKey.Length < 32)
                sKey = sKey.PadRight(32, '#');
            else
                sKey = sKey.Substring(0, 32);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(sKey);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

    }
}