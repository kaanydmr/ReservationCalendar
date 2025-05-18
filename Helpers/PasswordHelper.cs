using System;
using System.Security.Cryptography;
using System.Text;

namespace Project.Helpers
{
    public static class PasswordHelper
    {
        public static (string hash, string salt) HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                // Generate a salt from HMACSHA512 key
                byte[] salt = hmac.Key;

                // Compute the hash of the password with the salt
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Return the hash and the salt as Base64 strings
                return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
            }
        }
    }
}