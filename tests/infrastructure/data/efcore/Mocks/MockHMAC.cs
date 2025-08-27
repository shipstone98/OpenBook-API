using System;
using System.Security.Cryptography;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockHMAC : HMAC
{
    internal Action<byte[], int, int> _hashCoreAction;
    internal Func<byte[]> _hashFinalFunc;
    internal Action _initializeAction;

    public sealed override bool CanReuseTransform =>
        throw new NotImplementedException();

    public sealed override bool CanTransformMultipleBlocks =>
        throw new NotImplementedException();

    public sealed override byte[]? Hash => throw new NotImplementedException();
    public sealed override int HashSize => throw new NotImplementedException();

    public sealed override int InputBlockSize =>
        throw new NotImplementedException();

    public sealed override byte[] Key
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public sealed override int OutputBlockSize =>
        throw new NotImplementedException();

    public MockHMAC()
    {
        this._hashCoreAction = (_, _, _) =>
            throw new NotImplementedException();

        this._hashFinalFunc = () => throw new NotImplementedException();
        this._initializeAction = () => throw new NotImplementedException();
    }

    protected sealed override void Dispose(bool disposing) =>
        throw new NotImplementedException();

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    protected sealed override void HashCore(byte[] rgb, int ib, int cb) =>
        this._hashCoreAction(rgb, ib, cb);

    protected sealed override void HashCore(ReadOnlySpan<byte> source) =>
        throw new NotImplementedException();

    protected sealed override byte[] HashFinal() => this._hashFinalFunc();
    public sealed override void Initialize() => this._initializeAction();

    public sealed override String? ToString() =>
        throw new NotImplementedException();

    protected sealed override bool TryHashFinal(
        Span<byte> destination,
        out int bytesWritten
    ) =>
        throw new NotImplementedException();
}
