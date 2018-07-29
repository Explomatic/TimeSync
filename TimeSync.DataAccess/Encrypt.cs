using System;
using System.Text;
using System.Security.Cryptography;

namespace TimeSync.DataAccess
{
    class Encrypt : IEncrypt
    {
        private static readonly byte[] AdditionalEntropy = InitializeEntropy();

        private static byte[] InitializeEntropy()
        {
            return new byte[] { 1, 13, 1,4, 5, 234, 4, 2, 234, 56, 35, 234 };
        }

        public string EncryptText(string plainText)
        {
            var plainTextBytes = Encoding.ASCII.GetBytes(plainText);
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return Convert.ToBase64String(ProtectedData.Protect(plainTextBytes, AdditionalEntropy, DataProtectionScope.CurrentUser));
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public string DecryptText(string encryptedText)
        {
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return Convert.ToBase64String(ProtectedData.Protect(encryptedTextBytes, AdditionalEntropy, DataProtectionScope.CurrentUser));
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }
    }
}
