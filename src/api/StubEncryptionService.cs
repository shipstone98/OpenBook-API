using System;

using Shipstone.Extensions.Security;

namespace Shipstone.OpenBook.Api.WebApi;

internal sealed class StubEncryptionService : IEncryptionService
{
    String IEncryptionService.Decrypt(String encryptedData) => encryptedData;
    String IEncryptionService.Encrypt(String data) => data;
}
