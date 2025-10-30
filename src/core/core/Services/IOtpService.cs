using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.Core.Services;

internal interface IOtpService
{
    Task GenerateAsync(
        UserEntity user,
        Func<IMailService, UserEntity, int, CancellationToken, Task> mailSend,
        CancellationToken cancellationToken
    );

    Task ValidateAsync(
        UserEntity user,
        String otp,
        DateTime now,
        CancellationToken cancellationToken
    );
}
