using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core;

internal static class AuthorizationServiceExtensions
{
    internal static async Task AuthorizeAsync<TId>(
        this IAuthorizationService authorization,
        CreatableEntity<TId> entity,
        String policy,
        String exceptionMessage,
        CancellationToken cancellationToken
    )
        where TId : struct
    {
        try
        {
            await authorization.AuthorizeAsync(
                entity,
                policy,
                cancellationToken
            );
        }

        catch (ForbiddenException ex)
        {
            throw new ForbiddenException(exceptionMessage, ex);
        }
    }
}
