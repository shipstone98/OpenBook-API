using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web.Models.Account;

namespace Shipstone.OpenBook.Api.Web.Controllers;

[Route("/api/[controller]/[action]")]
internal sealed class AccountController(ILogger<AccountController> logger)
    : ControllerBase<AccountController>(logger)
{
    [ActionName("Authenticate")]
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> AuthenticateAsync(
        [FromServices] IAuthenticateHandler handler,
        [FromBody] AuthenticateRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);

        return this.AuthenticateAsyncCore(
            handler,
            request,
            cancellationToken
        );
    }

    private async Task<IActionResult> AuthenticateAsyncCore(
        IAuthenticateHandler handler,
        AuthenticateRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await handler.HandleAsync(
                request._emailAddress,
                request._password,
                cancellationToken
            );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed authentication for user {EmailAddress} - user not verified",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (IncorrectPasswordException ex)
        {
            this._logger.LogWarning(
                ex,
                "{TimeStamp}: Failed authentication for user {EmailAddress} - password not correct",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed authentication for user {EmailAddress} - email address not found",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed authentication for user {EmailAddress} - user not active",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} authenticated",
            DateTime.UtcNow,
            request._emailAddress
        );

        return this.NoContent();
    }
}
