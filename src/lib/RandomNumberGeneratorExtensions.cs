using System;
using System.Security.Cryptography;
using System.Text;

namespace Shipstone.Utilities.Security.Cryptography;

public static class RandomNumberGeneratorExtensions
{
    public static String GenerateOtp(
        this RandomNumberGenerator rng,
        int length
    )
    {
        ArgumentNullException.ThrowIfNull(rng);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(length, 0);

        Span<byte> bytes =
            length < 1024 ? stackalloc byte[length] : new byte[length];

        rng.GetNonZeroBytes(bytes);

        for (int i = 0; i < length; i ++)
        {
            bytes[i] = (byte) ((bytes[i] % 9) + '1');
        }

        return Encoding.ASCII.GetString(bytes);
    }
}
