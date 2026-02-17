using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web.Models.User;

namespace Shipstone.OpenBook.Api.Web.Areas.Administrator.Controllers;

internal sealed class UserController : ControllerBase<UserController>
{
    public UserController(ILogger<UserController> logger)
        : base(logger)
    { }

    [ActionName("List")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("/api/[area]/[controller]/[action]")]
    public Task<IActionResult> ListAsync(
        [FromServices] IUserListHandler handler,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        return this.ListAsyncCore(handler, cancellationToken);
    }

    private async Task<IActionResult> ListAsyncCore(
        IUserListHandler handler,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<IUser> users =
            await handler.HandleAsync(cancellationToken);

        Object? response = users.Select((u, _) => new RetrieveResponse(u));
        return this.Ok(response);
    }
}
