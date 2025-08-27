using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.Infrastructure.AuthenticationTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationServiceTest
{
    private const int _otpExpiryMinutes = 123456789;

    private readonly IAuthenticationService _authentication;
    private readonly MockRandomNumberGenerator _rng;

    public AuthenticationServiceTest()
    {
        IList<ServiceDescriptor> collection = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._countFunc = () => collection.Count;
        services._itemFunc = i => collection[i];
        services._getEnumeratorFunc = collection.GetEnumerator;

        services.AddOpenBookInfrastructureAuthentication(options =>
            options.OtpExpiryMinutes = AuthenticationServiceTest._otpExpiryMinutes);

        MockRandomNumberGenerator rng = new();
        services.AddSingleton<RandomNumberGenerator>(rng);
        IServiceProvider provider = new MockServiceProvider(services);

        this._authentication =
            provider.GetRequiredService<IAuthenticationService>();

        this._rng = rng;
    }

    [Fact]
    public async Task TestGenerateOtpAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.GenerateOtpAsync(
                    null!,
                    DateTime.MinValue,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public async Task TestGenerateOtpAsync_Valid()
    {
        // Arrange
        UserEntity user = new();
        DateTime now = DateTime.UtcNow;
        this._rng._getNonZeroBytesAction = _ => { };

        // Act
        await this._authentication.GenerateOtpAsync(
            user,
            now,
            CancellationToken.None
        );

        // Assert
        int otp = Int32.Parse(user.Otp!);
        Assert.False(otp < 100_000);
        Assert.True(otp < 1_000_000);

        Assert.Equal(
            now.AddMinutes(AuthenticationServiceTest._otpExpiryMinutes),
            user.OtpExpires!.Value
        );

        Assert.Equal(DateTimeKind.Utc, user.OtpExpires.Value.Kind);
        Assert.Equal(now, user.Updated);
        Assert.Equal(DateTimeKind.Utc, user.Updated.Kind);
    }
}
