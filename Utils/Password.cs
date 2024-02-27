using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Utils
{
    internal class Password
    {
        internal string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var algo = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = algo.GetBytes(20);

            var hashAndSalt = new byte[36];
            Array.Copy(salt, 0, hashAndSalt, 0, 16);
            Array.Copy(hash, 0, hashAndSalt, 16, 20);

            return Convert.ToBase64String(hashAndSalt);
        }

        internal bool VerifyPassword(string password, string hashedPassword) 
        { 
            var hashAndSaltBytes = Convert.FromBase64String(hashedPassword);

            var salt = new byte[16];
            Array.Copy(hashAndSaltBytes, 0, salt, 0, 16);

            var algo = new Rfc2898DeriveBytes(password, salt, 10000);
            var newHash = algo.GetBytes(20);

            return hashAndSaltBytes.Skip(16).SequenceEqual(newHash);
        }
    }
}
