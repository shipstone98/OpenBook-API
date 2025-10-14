using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Linq;

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
        Guid userId = this._claims.Id;

        UserEntity? user =
            await this._repository.Users.RetrieveAsync(
                userId,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("The current user could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The current user is not active.");
        }

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }
}
