using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthTray.Security.Test
{
    [TestClass]
    public class Crypto_Should
    {
        [TestMethod]
        public void Generate_Random_Salts()
        {
            const int length = 16;

            string salt = Crypto.GenerateSalt(length);

            Assert.IsFalse(string.IsNullOrWhiteSpace(salt), "Salt should not be a null or whitespace string.");
            Assert.IsTrue(salt.Length >= length, "Salt length should be >= the specified length.");
        }

        [TestMethod]
        public void Decrypt_A_String_Back_To_Its_Original_Plaintext()
        {
            const string plaintext = "this is a sensitive string";
            const string salt = "pepper";

            string ciphertext = Crypto.Encrypt(plaintext, salt);

            Assert.AreEqual(plaintext, Crypto.Decrypt(ciphertext, salt));
        }

        [TestMethod, ExpectedException(typeof(CryptographicException))]
        public void Throw_An_Exception_For_Incorrect_Salt()
        {
            const string plaintext = "this is another sensitive string";

            string ciphertext = Crypto.Encrypt(plaintext, "apple");
            string shouldntWork = Crypto.Decrypt(ciphertext, "orange");
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Throw_An_Exception_When_Decrypting_Non_Base64_Strings()
        {
            const string ciphertext = "this isn't a base64 string";

            string shouldntWork = Crypto.Decrypt(ciphertext, "salt");
        }
    }
}
