using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;

namespace Shipstone.OpenBook.Api.Core;

internal static class AuthorizationServiceExtensions
{
    internal static async Task AuthorizeAsync(
        this IAuthorizationService authorization,
        IResource resource,
        String policy,
        String exceptionMessage,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await authorization.AuthorizeAsync(
                resource,
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
