using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web.Models.User;

namespace Shipstone.OpenBook.Api.Web.Controllers;

internal sealed class UserController : ControllerBase<UserController>
{
    public UserController(ILogger<UserController> logger) : base(logger) { }

    [ActionName("Retrieve")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> RetrieveAsync(
        [FromServices] IUserRetrieveHandler handler,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        return this.RetrieveAsyncCore(handler, cancellationToken);
    }

    private async Task<IActionResult> RetrieveAsyncCore(
        IUserRetrieveHandler handler,
        CancellationToken cancellationToken
    )
    {
        IUser user = await handler.HandleAsync(cancellationToken);
        Object? response = new RetrieveResponse(user);
        return this.Ok(response);
    }
}
