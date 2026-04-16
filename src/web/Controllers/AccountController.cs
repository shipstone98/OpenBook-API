using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web.Models.Account;

namespace Shipstone.OpenBook.Api.Web.Controllers;

[Route("/api/[controller]/[action]")]
internal sealed class AccountController(ILogger<AccountController> logger)
    : ControllerBase<AccountController>(logger)
{
    [ActionName("Register")]
    [AllowUnregistered]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> RegisterAsync(
        [FromServices] IRegisterHandler handler,
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        return this.RegisterAsyncCore(handler, request, cancellationToken);
    }

    private async Task<IActionResult> RegisterAsyncCore(
        IRegisterHandler handler,
        RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        Claim? claim =
            this.HttpContext.User.Claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        if (!Guid.TryParse(claim?.Value, out Guid identityId))
        {
            return this.Unauthorized();
        }

        IUser user;

        try
        {
            user =
                await handler.HandleAsync(
                    identityId,
                    request._userName,
                    cancellationToken
                );
        }

        catch (ConflictException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed account creation for user {UserName} - user name taken",
                DateTime.UtcNow,
                request._userName
            );

            throw;
        }

        this._logger.LogInformation(
            "{TimeStamp}: Account created for user {UserName}",
            user.Created,
            user.UserName
        );

        return this.NoContent();
    }

    [ActionName("Unregister")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> UnregisterAsync(
        [FromServices] IUnregisterHandler handler,
        [FromServices] IClaimsService claims,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        return this.UnregisterAsyncCore(handler, claims, cancellationToken);
    }

    private async Task<IActionResult> UnregisterAsyncCore(
        IUnregisterHandler handler,
        IClaimsService claims,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await handler.HandleAsync(cancellationToken);
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed account deletion - user not active",
                DateTime.UtcNow
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {UserName} deleted account",
            DateTime.UtcNow,
            claims.User.UserName
        );

        return this.NoContent();
    }
}
