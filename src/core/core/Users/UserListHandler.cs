using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

namespace Shipstone.OpenBook.Api.Core.Users;

internal sealed class UserListHandler : IUserListHandler
{
    private readonly IRepository _repository;

    public UserListHandler(IRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this._repository = repository;
    }

    async Task<IReadOnlyPaginatedList<IUser>> IUserListHandler.HandleAsync(CancellationToken cancellationToken)
    {
        IReadOnlyPaginatedList<UserEntity> users =
            await this._repository.Users.ListAsync(cancellationToken);

        return await users.SelectAsync(
            (u, _, ct) => this._repository.RetrieveUserAsync(u, ct),
            cancellationToken
        );
    }
}
