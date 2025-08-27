using System;
using System.Security.Cryptography;

namespace Shipstone.Utilities.Security;

public sealed class ConcurrentRandomNumberGenerator : RandomNumberGenerator
{
    private readonly Object _locker;
    private readonly RandomNumberGenerator _rng;

    public ConcurrentRandomNumberGenerator(RandomNumberGenerator rng)
    {
        ArgumentNullException.ThrowIfNull(rng);
        this._locker = new();
        this._rng = rng;
    }

    protected sealed override void Dispose(bool disposing)
    {
        lock (this._locker)
        {
            this._rng.Dispose();
        }
    }

    public sealed override bool Equals(Object? obj)
    {
        lock (this._locker)
        {
            return this._rng.Equals(obj);
        }
    }

    public sealed override void GetBytes(byte[] data)
    {
        lock (this._locker)
        {
            this._rng.GetBytes(data);
        }
    }

    public sealed override void GetBytes(byte[] data, int offset, int count)
    {
        lock (this._locker)
        {
            this._rng.GetBytes(data, offset, count);
        }
    }

    public sealed override void GetBytes(Span<byte> data)
    {
        lock (this._locker)
        {
            this._rng.GetBytes(data);
        }
    }

    public sealed override int GetHashCode()
    {
        lock (this._locker)
        {
            return this._rng.GetHashCode();
        }
    }

    public sealed override void GetNonZeroBytes(byte[] data)
    {
        lock (this._locker)
        {
            this._rng.GetNonZeroBytes(data);
        }
    }

    public sealed override void GetNonZeroBytes(Span<byte> data)
    {
        lock (this._locker)
        {
            this._rng.GetNonZeroBytes(data);
        }
    }

    public sealed override String? ToString()
    {
        lock (this._locker)
        {
            return this._rng.ToString();
        }
    }
}
