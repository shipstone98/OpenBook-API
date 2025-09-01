using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostCreateHandler : IPostCreateHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public PostCreateHandler(IRepository repository, IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IPost> HandleAsync(
        PostBuilder builder,
        CancellationToken cancellationToken
    )
    {
        DateTime now = DateTime.UtcNow;
        Nullable<long> parentId = builder.ParentId;

        PostEntity post = new PostEntity
        {
            Body = builder.Body,
            Created = now,
            CreatorId = this._claims.Id,
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

        return new Post(
            post,
            this._claims.EmailAddress,
            this._claims.UserName
        );
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
