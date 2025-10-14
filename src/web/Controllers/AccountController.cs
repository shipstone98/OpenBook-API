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
using Shipstone.OpenBook.Api.Core.Users;
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

    [ActionName("OtpAuthenticate")]
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> OtpAuthenticateAsync(
        [FromServices] IOtpAuthenticateHandler handler,
        [FromBody] OtpAuthenticateRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);

        return this.OtpAuthenticateAsyncCore(
            handler,
            request,
            cancellationToken
        );
    }

    private async Task<IActionResult> OtpAuthenticateAsyncCore(
        IOtpAuthenticateHandler handler,
        OtpAuthenticateRequest request,
        CancellationToken cancellationToken
    )
    {
        IAuthenticateResult result;

        try
        {
            result =
                await handler.HandleAsync(
                    request._emailAddress,
                    request._otp,
                    cancellationToken
                );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed OTP authentication for user {EmailAddress} - OTP not valid or expired",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed OTP authentication for user {EmailAddress} - email address not found",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed OTP authentication for user {EmailAddress} - user not active",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} authenticated using OTP",
            DateTime.UtcNow,
            request._emailAddress
        );

        Object? response = new OtpAuthenticateResponse(result);
        return this.Ok(response);
    }

    [ActionName("Register")]
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
        IUser user;

        try
        {
            user =
                await handler.HandleAsync(
                    request._emailAddress,
                    request._userName,
                    request._forename,
                    request._surname,
                    request._born,
                    cancellationToken
                );
        }

        catch (ConflictException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed account creation for user {EmailAddress} - email address and/or user name taken",
                DateTime.UtcNow,
                request._emailAddress
            );

            throw;
        }

        this._logger.LogInformation(
            "{TimeStamp}: Account created for user {EmailAddress}",
            user.Created,
            user.EmailAddress
        );

        return this.NoContent();
    }
}
