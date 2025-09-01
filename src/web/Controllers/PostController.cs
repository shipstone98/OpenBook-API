using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Web.Models;
using Shipstone.OpenBook.Api.Web.Models.Post;

namespace Shipstone.OpenBook.Api.Web.Controllers;

internal sealed class PostController : ControllerBase<PostController>
{
    public PostController(ILogger<PostController> logger) : base(logger) { }

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

    [ActionName("Retrieve")]
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
