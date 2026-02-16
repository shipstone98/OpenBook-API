using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Web.Models;
using Shipstone.OpenBook.Api.Web.Models.Post;

namespace Shipstone.OpenBook.Api.Web.Controllers;

internal sealed class PostController(ILogger<PostController> logger)
    : ControllerBase<PostController>(logger)
{
    [ActionName("Aggregate")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Route("/api/[controller]/[action]")]
    public Task<IActionResult> AggregateAsync(
        [FromServices] IPostAggregateHandler handler,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        return this.AggregateAsyncCore(handler, cancellationToken);
    }

    private async Task<IActionResult> AggregateAsyncCore(
        IPostAggregateHandler handler,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<IPost> posts =
            await handler.HandleAsync(cancellationToken);

        Object? response = posts.Select((p, _) => new RetrieveResponse(p));
        return this.Ok(response);
    }

    [ActionName("Children")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("/api/[controller]/[action]")]
    public Task<IActionResult> ChildrenAsync(
        [FromServices] IPostListHandler handler,
        [FromQuery] long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        return this.ChildrenAsyncCore(handler, id, cancellationToken);
    }

    private async Task<IActionResult> ChildrenAsyncCore(
        IPostListHandler handler,
        long id,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<IPost> posts =
            await handler.HandleAsync(id, cancellationToken);

        Object? response = posts.Select((p, _) => new RetrieveResponse(p));
        return this.Ok(response);
    }

    [ActionName("Create")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> CreateAsync(
        [FromServices] IPostCreateHandler handler,
        [FromServices] IClaimsService claims,
        [FromBody] CreateRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(request);

        return this.CreateAsyncCore(
            handler,
            claims,
            request,
            cancellationToken
        );
    }

    private async Task<IActionResult> CreateAsyncCore(
        IPostCreateHandler handler,
        IClaimsService claims,
        CreateRequest request,
        CancellationToken cancellationToken
    )
    {
        IPost post;

        try
        {
            post =
                await handler.HandleAsync(
                    new PostBuilder
                    {
                        Body = request._body,
                        ParentId = request._parentId
                    },
                    cancellationToken
                );
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed to create post for user {EmailAddress} - parent post not found",
                DateTime.UtcNow,
                claims.EmailAddress
            );

            throw;
        }

        long id = post.Id;

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} created post {Id}",
            post.Created,
            post.CreatorEmailAddress,
            id
        );

        Object? response = new IdResponse<long>(id);

        return this.CreatedAtAction(
            "Retrieve",
            new
            {
                Id = id
            },
            response
        );
    }

    [ActionName("Delete")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteAsync(
        [FromServices] IPostDeleteHandler handler,
        [FromServices] IClaimsService claims,
        [FromQuery] long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        return this.DeleteAsyncCore(handler, claims, id, cancellationToken);
    }

    private async Task<IActionResult> DeleteAsyncCore(
        IPostDeleteHandler handler,
        IClaimsService claims,
        long id,
        CancellationToken cancellationToken
    )
    {
        IPost post;

        try
        {
            post =
                await handler.HandleAsync(
                    id,
                    Policies.ResourceOwner,
                    cancellationToken
                );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to delete post {Id} - not authorized",
                DateTime.UtcNow,
                claims.EmailAddress,
                id
            );

            throw;
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to delete post {Id} - not found",
                DateTime.UtcNow,
                claims.EmailAddress,
                id
            );

            throw;
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} deleted post {Id}",
            post.Updated,
            post.CreatorEmailAddress,
            post.Id
        );

        return this.NoContent();
    }

    [ActionName("List")]
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("/api/[controller]/[action]")]
    public Task<IActionResult> ListAsync(
        [FromServices] IPostListHandler handler,
        [FromQuery(Name = "creator")] String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(userName);
        return this.ListAsyncCore(handler, userName, cancellationToken);
    }

    private async Task<IActionResult> ListAsyncCore(
        IPostListHandler handler,
        String userName,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<IPost> posts =
            await handler.HandleAsync(userName, cancellationToken);

        Object? response = posts.Select((p, _) => new RetrieveResponse(p));
        return this.Ok(response);
    }

    [ActionName("Retrieve")]
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RetrieveAsync(
        [FromServices] IPostRetrieveHandler handler,
        [FromQuery] long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        return this.RetrieveAsyncCore(handler, id, cancellationToken);
    }

    private async Task<IActionResult> RetrieveAsyncCore(
        IPostRetrieveHandler handler,
        long id,
        CancellationToken cancellationToken
    )
    {
        IPost post = await handler.HandleAsync(id, cancellationToken);
        Object? response = new RetrieveResponse(post);
        return this.Ok(response);
    }
}
