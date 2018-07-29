using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.DataAccess
{
    public interface IEncrypt
    {
        string EncryptText(string plainText);
        string DecryptText(string encryptedText);
    }
}
