using System;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockClaimsService : IClaimsService
{
    internal Func<Guid> _idFunc;

    Guid IClaimsService.Id => this._idFunc();

    public MockClaimsService() =>
        this._idFunc = () => throw new NotImplementedException();
}
