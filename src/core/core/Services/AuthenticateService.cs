using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Services;

internal sealed class AuthenticateService : IAuthenticateService
{
    private readonly IAuthenticationService _authentication;
    private readonly IRepository _repository;

    public AuthenticateService(
        IRepository repository,
        IAuthenticationService authentication
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        this._authentication = authentication;
        this._repository = repository;
    }

    async Task<IAuthenticateResult> IAuthenticateService.AuthenticateAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        Guid userId = user.Id;

        IAsyncEnumerable<String> roles =
            await this._repository.RetrieveRolesAsync(
                userId,
                cancellationToken
            );

        IEnumerable<String> roleCollection =
            await roles.ToListAsync(cancellationToken);

        IAuthenticateResult result =
            await this._authentication.AuthenticateAsync(
                user,
                roleCollection,
                now,
                cancellationToken
            );

        await this._repository.UserRefreshTokens.CreateAsync(
            new UserRefreshTokenEntity
            {
                Created = now,
                Expires = result.RefreshTokenExpires,
                Updated = now,
                UserId = userId,
                Value = result.RefreshToken
            },
            cancellationToken
        );

        await this._repository.SaveAsync(cancellationToken);
        return result;
    }
}
