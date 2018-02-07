using System;
using System.Collections.Generic;
using System.Text;
using NetSHA;

namespace ESD.WITS.Helper
{
    public class HashConverter
    {
        public static string CalculateHash(string clearTextPassword, string salt)
        {
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);

            byte[] hash = SHA256.MessageSHA256(saltedHashBytes);

            return Convert.ToBase64String(hash);
        }
    }
}
