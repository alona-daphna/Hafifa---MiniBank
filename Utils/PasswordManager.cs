using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Utils
{
    internal class PasswordManager
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

        internal string GetPasswordInput()
        {
            var password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace)
                {
                    password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                } 
                else if (key.Key != ConsoleKey.Enter && !char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return password;
        }
    }
}
