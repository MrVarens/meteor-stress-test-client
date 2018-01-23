using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace Meteor.StressTest
{
    public static class Utils
    {
        private static Random RNG = new Random();

        public static string SHA256(string text)
        {
            SHA256Managed manager = new SHA256Managed();

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = manager.ComputeHash(bytes);

            string hashString = string.Empty;
            foreach (byte x in hash)
                hashString += String.Format("{0:x2}", x);

            return hashString;
        }

        public static string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length).Select(x => pool[RNG.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
    }
}
