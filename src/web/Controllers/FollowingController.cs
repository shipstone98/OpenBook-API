using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Web.Models.Following;

namespace Shipstone.OpenBook.Api.Web.Controllers;

internal sealed class FollowingController(ILogger<FollowingController> logger)
    : ControllerBase<FollowingController>(logger)
{
    [ActionName("Create")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> CreateAsync(
        [FromServices] IFollowingCreateHandler handler,
        [FromServices] IClaimsService claims,
        [FromBody] CreateRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(claims);

        return this.CreateAsyncCore(
            handler,
            claims,
            request,
            cancellationToken
        );
    }

    private async Task<IActionResult> CreateAsyncCore(
        IFollowingCreateHandler handler,
        IClaimsService claims,
        CreateRequest request,
        CancellationToken cancellationToken
    )
    {
        IFollowing following;

        try
        {
            following =
                await handler.HandleAsync(
                    request._userName,
                    request._isSubscribed,
                    cancellationToken
                );
        }

        catch (ConflictException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to follow user {UserName} - user is already followed",
                DateTime.UtcNow,
                claims.EmailAddress,
                request._userName
            );

            throw;
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to follow user {UserName} - user is current user",
                DateTime.UtcNow,
                claims.EmailAddress,
                request._userName
            );

            throw;
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to follow user {UserName} - user not found",
                DateTime.UtcNow,
                claims.EmailAddress,
                request._userName
            );

            throw;
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to follow user {UserName} - user not active",
                DateTime.UtcNow,
                claims.EmailAddress,
                request._userName
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} followed user {UserName}",
            following.Followed,
            following.FollowerEmailAddress,
            following.FolloweeName
        );

        return this.NoContent();
    }

    [ActionName("Delete")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> DeleteAsync(
        [FromServices] IFollowingDeleteHandler handler,
        [FromServices] IClaimsService claims,
        [FromQuery] String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(userName);

        return this.DeleteAsyncCore(
            handler,
            claims,
            userName,
            cancellationToken
        );
    }

    private async Task<IActionResult> DeleteAsyncCore(
        IFollowingDeleteHandler handler,
        IClaimsService claims,
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(userName);
        IFollowing following;

        try
        {
            following = await handler.HandleAsync(userName, cancellationToken);
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to unfollow user {UserName} - user is not followed",
                DateTime.UtcNow,
                claims.EmailAddress,
                userName
            );

            throw;
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: User {EmailAddress} failed to unfollow user {UserName} - user not active",
                DateTime.UtcNow,
                claims.EmailAddress,
                userName
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} unfollowed user {UserName}",
            DateTime.UtcNow,
            following.FollowerEmailAddress,
            following.FolloweeName
        );

        return this.NoContent();
    }
}
