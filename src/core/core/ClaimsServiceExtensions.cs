using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core;

internal static class ClaimsServiceExtensions
{
    internal static async Task<UserEntity> RetrieveActiveUserAsync(
        this IClaimsService claims,
        IRepository repository,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await repository.Users.RetrieveAsync(
                claims.Id,
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

        return user;
    }
}
