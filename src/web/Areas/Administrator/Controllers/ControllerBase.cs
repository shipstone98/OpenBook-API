using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Areas.Administrator.Controllers;

[Area(Internals._areaAdministrator)]
[Authorize(Roles = $"{Roles.Administrator},{Roles.SystemAdministrator}")]
[Route("/api/[area]/[controller]")]
internal abstract class ControllerBase<TCategoryName>
    : Web.Controllers.ControllerBase<TCategoryName>
    where TCategoryName : notnull, ControllerBase<TCategoryName>
{
    private protected ControllerBase(ILogger<TCategoryName> logger)
        : base(logger)
    { }
}
