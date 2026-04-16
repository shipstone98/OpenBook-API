using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Users;

internal sealed class UserRetrieveHandler : IUserRetrieveHandler
{
    private readonly IRepository _repository;

    public UserRetrieveHandler(IRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this._repository = repository;
    }

    private async Task<IUser> HandleAsync(
        Guid identityId,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await this._repository.Users.RetrieveForIdentityIdAsync(
                identityId,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose identity ID matches the provided identity ID could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose identity ID matches the provided identity ID is not active.");
        }

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }

    Task<IUser> IUserRetrieveHandler.HandleAsync(
        Guid identityId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(identityId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (identityId)} is equal to Guid.Empty.",
                nameof (identityId)
            );
        }

        return this.HandleAsync(identityId, cancellationToken);
    }
}
