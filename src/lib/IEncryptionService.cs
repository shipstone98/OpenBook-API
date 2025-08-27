using System;

namespace Shipstone.Extensions.Security;

public interface IEncryptionService
{
    String Decrypt(String encryptedData);
    String Encrypt(String data);
}
