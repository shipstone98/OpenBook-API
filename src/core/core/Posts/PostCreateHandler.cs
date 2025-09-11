using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostCreateHandler : IPostCreateHandler
{
    private readonly IClaimsService _claims;
    private readonly INotificationService _notification;
    private readonly IRepository _repository;

    public PostCreateHandler(
        IRepository repository,
        IClaimsService claims,
        INotificationService notification
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(notification);
        this._claims = claims;
        this._notification = notification;
        this._repository = repository;
    }

    private async Task<IPost> HandleAsync(
        PostBuilder builder,
        CancellationToken cancellationToken
    )
    {
        DateTime now = DateTime.UtcNow;
        Nullable<long> parentId = builder.ParentId;
        Guid creatorId = this._claims.Id;

        PostEntity post = new PostEntity
        {
            Body = builder.Body,
            Created = now,
            CreatorId = creatorId,
            ParentId = parentId,
            Updated = now
        };

        try
        {
            await this._repository.Posts.CreateAsync(post, cancellationToken);
            await this._repository.SaveAsync(cancellationToken);
        }

        catch
        {
            if (parentId.HasValue)
            {
                throw new NotFoundException("A post whose ID matches the parent ID of the provided post builder could not be found.");
            }

            throw;
        }

        String creatorName = this._claims.UserName;

        await this._notification.NotifyAllSubscribedFollowersAsync(
            this._repository,
            creatorId,
            creatorName,
            post.Id,
            cancellationToken
        );

        return new Post(post, this._claims.EmailAddress, creatorName);
    }

    Task<IPost> IPostCreateHandler.HandleAsync(
        PostBuilder builder,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        return this.HandleAsync(builder, cancellationToken);
    }
}
