using System;
using System.Security.Cryptography;
using System.Text;

namespace Params
{
    public static class PasswordService
    {
        public static string GetPasswordHash(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", string.Empty);
        }
    }
}
