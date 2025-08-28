using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockAuthenticationService : IAuthenticationService
{
    internal Func<UserEntity, IEnumerable<String>, DateTime, IAuthenticateResult> _authenticateAction;
    internal Action<UserEntity, DateTime> _generateOtpAction;

    public MockAuthenticationService()
    {
        this._authenticateAction = (_, _, _) =>
            throw new NotImplementedException();

        this._generateOtpAction = (_, _) =>
            throw new NotImplementedException();
    }

    Task<IAuthenticateResult> IAuthenticationService.AuthenticateAsync(
        UserEntity user,
        IEnumerable<String> roles,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        IAuthenticateResult result =
            this._authenticateAction(user, roles, now);

        return Task.FromResult(result);
    }

    Task IAuthenticationService.GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        this._generateOtpAction(user, now);
        return Task.CompletedTask;
    }
}
