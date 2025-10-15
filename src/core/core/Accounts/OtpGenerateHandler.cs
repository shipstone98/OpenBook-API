using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class OtpGenerateHandler : IOtpGenerateHandler
{
    private readonly IOtpService _otp;
    private readonly IRepository _repository;

    public OtpGenerateHandler(IRepository repository, IOtpService otp)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(otp);
        this._otp = otp;
        this._repository = repository;
    }

    private async Task HandleAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

        await this._otp.GenerateAsync(user, cancellationToken);
    }

    Task IOtpGenerateHandler.HandleAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        return this.HandleAsync(emailAddress, cancellationToken);
    }
}
