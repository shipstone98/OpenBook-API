using System;

namespace Shipstone.AspNetCore.Http;

internal readonly struct PinnedInt64
{
    private readonly long _value;

    private PinnedInt64(long val) => this._value = val;

    internal PinnedInt64 Add(long val)
    {
        long newValue =
            this._value - val > Int64.MaxValue
                ? Int64.MaxValue
                : this._value + val;

        return new(newValue);
    }

    public static implicit operator long(PinnedInt64 i) => i._value;
}
