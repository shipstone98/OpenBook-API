using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Passwords;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web.Models.Password;

namespace Shipstone.OpenBook.Api.Web.Controllers;

[Route("/api/[controller]/[action]")]
internal sealed class PasswordController : ControllerBase<PasswordController>
{
    public PasswordController(ILogger<PasswordController> logger)
        : base(logger)
    { }

    [ActionName("Set")]
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> SetAsync(
        [FromServices] IPasswordSetHandler handler,
        [FromBody] SetRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        return this.SetAsyncCore(handler, request, cancellationToken);
    }

    private async Task<IActionResult> SetAsyncCore(
        IPasswordSetHandler handler,
        SetRequest request,
        CancellationToken cancellationToken
    )
    {
        IUser user;

        try
        {
            user =
                await handler.HandleAsync(
                    request._emailAddress,
                    request._otp,
                    request._password,
                    cancellationToken
                );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - OTP not valid or expired",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - email address not found",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - user not active",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} set password",
            user.Updated,
            user.EmailAddress
        );

        return this.NoContent();
    }
}
