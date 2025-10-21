using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Services;

internal interface IAuthenticateService
{
    Task<IAuthenticateResult> AuthenticateAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    );
}
