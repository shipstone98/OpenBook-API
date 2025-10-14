using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Services;

internal sealed class OtpService : IOtpService
{
    private readonly IRepository _repository;

    public OtpService(IRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this._repository = repository;
    }

    async Task IOtpService.ValidateOtpAsync(
        UserEntity user,
        String otp,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        String? userOtp = user.Otp;
        Nullable<DateTime> userOtpExpires = user.OtpExpires;

        if (userOtp is null || !String.Equals(userOtp, otp))
        {
            throw new ForbiddenException("The provided OTP does not match the OTP for the user whose email address matches the provided email address.");
        }

        Nullable<DateTime> otpExpires = userOtpExpires;
        user.Otp = null;
        user.OtpExpires = null;
        user.Updated = now;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        if (
            !otpExpires.HasValue
            || DateTime.Compare(now, otpExpires.Value) > 0
        )
        {
            throw new ForbiddenException("The provided OTP has expired.");
        }
    }
}
