using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

using Shipstone.Extensions.Security;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Services;

internal sealed class NormalizationService : INormalizationService
{
    private readonly Encoding _encoding;
    private readonly EncryptionOptions _encryptionOptions;

    public NormalizationService(IOptions<EncryptionOptions>? encryptionOptions)
    {
        this._encoding = Encoding.UTF8;
        this._encryptionOptions = encryptionOptions?.Value ?? new();
    }

    String INormalizationService.Normalize(String s)
    {
        ArgumentNullException.ThrowIfNull(s);
        byte[] key = Convert.FromBase64String(this._encryptionOptions.Key);
        using HMAC hmac = new HMACSHA256(key);
        s = s.ToUpperInvariant();
        byte[] bytes = this._encoding.GetBytes(s);
        ReadOnlySpan<byte> hash = hmac.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
