using System;
using System.Text;
using System.Security.Cryptography;

namespace HealthTray.Security
{
    /// <summary>
    /// Provides string-based encryption/decryption routines using Microsoft's
    /// <see cref="ProtectedData"/> class and underlying Windows Data Protection API (DPAPI).
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// Encrypts the given string and returns base64-encoded ciphertext.
        /// Uses <see cref="DataProtectionScope.CurrentUser"/> so that only the user
        /// who encrypted the string can later decrypt it.
        /// </summary>
        /// <param name="plaintext">The string of unencrypted sensitive data to encrypt.</param>
        /// <param name="salt">Additional input used to make this encryption unique.</param>
        public static string Encrypt(string plaintext, string salt)
        {
            if (salt == null) throw new ArgumentNullException(nameof(salt));

            byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(plaintext);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] encryptedBytes = ProtectedData.Protect(bytesToEncrypt, saltBytes, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts the given base64-encoded ciphertext and returns a string of plaintext.
        /// </summary>
        /// <param name="ciphertext">The string of encrypted ciphertext to decrypt.</param>
        /// <param name="salt">The salt that was used to encrypt this ciphertext.</param>
        public static string Decrypt(string ciphertext, string salt)
        {
            if (salt == null) throw new ArgumentNullException(nameof(salt));

            byte[] encryptedBytes = Convert.FromBase64String(ciphertext);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Generates and returns a random salt of at least the given length.
        /// The same salt that was used to encrypt a string must be used to decrypt it.
        /// </summary>
        /// <param name="howManyBytes">
        /// The number of bytes to use for the salt (recommended to be at least 16).
        /// <see>https://stackoverflow.com/questions/9619727/how-long-should-a-salt-be-to-make-it-infeasible-to-attempt-dictionary-attacks</see>
        /// </param>
        public static string GenerateSalt(int howManyBytes)
        {
            if (howManyBytes <= 0) throw new ArgumentException("Salt must be > 0 bytes.", nameof(howManyBytes));
            byte[] saltBytes = new byte[howManyBytes];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }
    }
}
