using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web.Models.User;

namespace Shipstone.OpenBook.Api.Web.Controllers;

internal sealed class UserController(ILogger<UserController> logger)
    : ControllerBase<UserController>(logger)
{
    [ActionName("Retrieve")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public IActionResult Retrieve([FromServices] IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        Object? response = new RetrieveResponse(claims.User);
        return this.Ok(response);
    }
}
