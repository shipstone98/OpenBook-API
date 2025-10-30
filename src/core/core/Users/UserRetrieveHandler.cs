using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Users;

internal sealed class UserRetrieveHandler : IUserRetrieveHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public UserRetrieveHandler(IRepository repository, IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    async Task<IUser> IUserRetrieveHandler.HandleAsync(CancellationToken cancellationToken)
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }
}
