using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockUserRetrieveHandler : IUserRetrieveHandler
{
    internal Func<Guid, IUser> _handleFunc;

    public MockUserRetrieveHandler() =>
        this._handleFunc = _ => throw new NotImplementedException();

    Task<IUser> IUserRetrieveHandler.HandleAsync(
        Guid identityId,
        CancellationToken cancellationToken
    )
    {
        IUser user = this._handleFunc(identityId);
        return Task.FromResult(user);
    }
}
