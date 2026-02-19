using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;

namespace Shipstone.OpenBook.Api.Web.Areas.Administrator.Controllers;

internal sealed class PostController : ControllerBase<PostController>
{
    public PostController(ILogger<PostController> logger) : base(logger) { }

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
                    Policies.Administrator,
                    cancellationToken
                );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Administrator {EmailAddress} failed to delete post with ID {Id} - not authorized",
                DateTime.UtcNow,
                claims.EmailAddress,
                id
            );

            return this.Forbid();
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Administrator {EmailAddress} failed to delete post with ID {Id} - ID not found",
                DateTime.UtcNow,
                claims.EmailAddress,
                id
            );

            return this.NotFound();
        }

        this._logger.LogInformation(
            "{TimeStamp}: Administrator {EmailAddress} deleted post with ID {Id}",
            post.Updated,
            claims.EmailAddress,
            id
        );

        return this.NoContent();
    }
}
