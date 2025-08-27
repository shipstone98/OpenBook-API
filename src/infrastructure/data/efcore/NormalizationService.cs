using System;
using System.Security.Cryptography;
using System.Text;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

internal sealed class NormalizationService : INormalizationService
{
    private readonly Encoding _encoding;
    private readonly HMAC _hmac;

    public NormalizationService(HMAC hmac)
    {
        ArgumentNullException.ThrowIfNull(hmac);
        this._encoding = Encoding.UTF8;
        this._hmac = hmac;
    }

    String INormalizationService.Normalize(String s)
    {
        s = s.ToUpperInvariant();
        byte[] bytes = this._encoding.GetBytes(s);
        ReadOnlySpan<byte> hash = this._hmac.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
