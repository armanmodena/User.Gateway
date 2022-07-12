using System;
using System.Security.Cryptography;
using System.Text;

namespace User.Gateway.Utils
{
    public class Hash
    {
        public static string EncryptSHA2(string text)
        {
            var sha2 = SHA512.Create();
            sha2.ComputeHash(Encoding.UTF8.GetBytes(text));
            var hash = BitConverter.ToString(sha2.Hash).Replace("-", "");
            return hash;
        }

        public static string EncryptMD5(string text)
        {
            var md5 = MD5.Create();
            md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            var hash = BitConverter.ToString(md5.Hash).Replace("-", "");
            return hash;
        }
    }
}
