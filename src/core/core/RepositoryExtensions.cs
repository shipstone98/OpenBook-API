using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core;

internal static class RepositoryExtensions
{
    internal static async Task<UserEntity> RetrieveActiveUserAsync(
        this IRepository repository,
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose email address matches the provided email address could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose email address matches the provided email address is not active.");
        }

        return user;
    }

    internal static async Task<UserEntity> RetrieveActiveUserForNameAsync(
        this IRepository repository,
        String userName,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await repository.Users.RetrieveForNameAsync(
                userName,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose name matches the provided user name could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose name matches the provided user name is not active.");
        }

        return user;
    }

    internal static async Task<IPost> RetrievePostAsync(
        this IRepository repository,
        IClaimsService claims,
        PostEntity post,
        CancellationToken cancellationToken
    )
    {
        Guid creatorId = post.CreatorId;
        String creatorEmailAddress;
        String creatorName;

        if (claims.IsAuthenticated && Guid.Equals(creatorId, claims.Id))
        {
            creatorEmailAddress = claims.EmailAddress;
            creatorName = claims.UserName;
        }

        else
        {
            UserEntity? creator =
                await repository.Users.RetrieveAsync(
                    creatorId,
                    cancellationToken
                );

            if (creator is null)
            {
                throw new NotFoundException("A user whose ID matches the creator ID of the post whose ID matches the provided ID could not be found.");
            }

            creatorEmailAddress = creator.EmailAddress;
            creatorName = creator.UserName;
        }

        return new Post(post, creatorEmailAddress, creatorName);
    }

    internal static async Task<IAsyncEnumerable<String>> RetrieveRolesAsync(
        this IRepository repository,
        Guid userId,
        CancellationToken cancellationToken
    )
    {

        IEnumerable<UserRoleEntity> userRoles =
            await repository.UserRoles.ListForUserAsync(
                userId,
                cancellationToken
            );

        return userRoles
            .SelectAsync(
                async (ur, ct) =>
                {
                    RoleEntity? role =
                        await repository.Roles.RetrieveAsync(ur.RoleId, ct);

                    return role?.Name;
                },
                cancellationToken
            )
            .WithoutNullAsync(cancellationToken);
    }

    internal static async Task<IUser> RetrieveUserAsync(
        this IRepository repository,
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        IAsyncEnumerable<String> roles =
            await repository.RetrieveRolesAsync(user.Id, cancellationToken);

        IReadOnlySet<String> roleSet =
            await roles.ToSortedSetAsync(null, cancellationToken);

        return new User(user, roleSet);
    }
}
