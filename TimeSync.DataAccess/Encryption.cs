using System;
using System.Text;
using System.Security.Cryptography;

namespace TimeSync.DataAccess
{
    public class Encryption : IEncryption
    {
        private static readonly byte[] AdditionalEntropy = InitializeEntropy();

        private static byte[] InitializeEntropy()
        {
            return new byte[] { 1, 13, 1,4, 5, 234, 4, 2, 234, 56, 35, 234 };
        }

        //TODO: Find a way to propagate erros from here to viewmodel to popup.

        public string EncryptText(string plainText)
        {
            var plainTextBytes = Encoding.ASCII.GetBytes(plainText);

            // Encryption the data using DataProtectionScope.CurrentUser. The result can be decrypted
            //  only by the same current user.
            return Convert.ToBase64String(ProtectedData.Protect(plainTextBytes, AdditionalEntropy, DataProtectionScope.CurrentUser));
        }

        public string DecryptText(string encryptedText)
        {
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);

            // Encryption the data using DataProtectionScope.CurrentUser. The result can be decrypted
            //  only by the same current user.
            return Convert.ToBase64String(ProtectedData.Protect(encryptedTextBytes, AdditionalEntropy, DataProtectionScope.CurrentUser));
        }
    }
}
