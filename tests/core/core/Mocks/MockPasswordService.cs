using System;

using Shipstone.Extensions.Identity;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockPasswordService : IPasswordService
{
    internal Func<String, String> _hashFunc;
    internal Func<String, String, bool> _verifyFunc;

    public MockPasswordService()
    {
        this._hashFunc = _ => throw new NotImplementedException();
        this._verifyFunc = (_, _) => throw new NotImplementedException();
    }

    String IPasswordService.Hash(String password) => this._hashFunc(password);

    bool IPasswordService.Verify(String passwordHash, String password) =>
        this._verifyFunc(passwordHash, password);
}
