using System;
using Microsoft.Extensions.Options;

namespace Shipstone.Extensions.Security;

public class EncryptionOptions : IOptions<EncryptionOptions>
{
    private byte[] _key;
    private String _keyString;

    public String Key
    {
        get => this._keyString;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            byte[] key;

            try
            {
                key = Convert.FromBase64String(value);
            }

            catch (FormatException ex)
            {
                throw new FormatException(
                    $"{nameof (value)} is not in the correct format.",
                    ex
                );
            }

            this._key = key;
            this._keyString = value;
        }
    }

    EncryptionOptions IOptions<EncryptionOptions>.Value => this;

    public EncryptionOptions()
    {
        String keyString = String.Empty;
        this._key = Convert.FromBase64String(keyString);
        this._keyString = keyString;
    }
}
