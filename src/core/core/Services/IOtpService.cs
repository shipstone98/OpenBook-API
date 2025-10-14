using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Services;

internal interface IOtpService
{
    Task ValidateAsync(
        UserEntity user,
        String otp,
        DateTime now,
        CancellationToken cancellationToken
    );
}
