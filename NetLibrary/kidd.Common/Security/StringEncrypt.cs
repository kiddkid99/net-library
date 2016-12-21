using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace kidd.Common.Security
{
    /// <summary>
    /// 字串加密演算法類別
    /// </summary>
    public static class StringEncrypt
    {
        /// <summary>
        /// 使用 DES 加密演算法加密字串，並轉成
        /// </summary>
        /// <param name="source">字串</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesEncryptBase64(string source, string key, string iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] des_key = Encoding.ASCII.GetBytes(key);
            byte[] des_iv = Encoding.ASCII.GetBytes(iv);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(source);

            des.Key = des_key;
            des.IV = des_iv;
            string encrypt = "";
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(dataByteArray, 0, dataByteArray.Length);
                cs.FlushFinalBlock();
                encrypt = Convert.ToBase64String(ms.ToArray());
            }
            return encrypt;
        }


        /// <summary>
        /// 使用 DES 加密演算法解密字串
        /// </summary>
        /// <param name="encrypt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesDecryptBase64(string encrypt, string key, string iv)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] des_key = Encoding.ASCII.GetBytes(key);
                byte[] des_iv = Encoding.ASCII.GetBytes(iv);
                des.Key = des_key;
                des.IV = des_iv;

                byte[] dataByteArray = Convert.FromBase64String(encrypt);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DecryptExcepion(ex.ToString());
            }
        }

        public static string Md5Encrypt(string source)
        {
            using (MD5CryptoServiceProvider md5Hash = new MD5CryptoServiceProvider())
            {

                // Convert the input string to a byte array and compute the hash. 
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                // Create a new Stringbuilder to collect the bytes 
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data  
                // and format each one as a hexadecimal string. 
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string. 
                return sBuilder.ToString();

            }
        }
    }

    public class DecryptExcepion : Exception
    {
        public DecryptExcepion(string message)
            : base(message)
        {
        }
    }
}

