using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Model.Helpers
{
    public static class EncryptionHelpers
    {
        private const int _saltSize = 8;

        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[_saltSize];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string HashPassword(string clearPassword, string salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            string saltAndPwd = String.Concat(salt, clearPassword);
            byte[] hashedPwd = algorithm.ComputeHash(Encoding.ASCII.GetBytes(saltAndPwd));
            return Convert.ToBase64String(hashedPwd);
        }
    }
}
