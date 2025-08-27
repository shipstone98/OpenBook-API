using System;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Authentication;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationOptionsTest
{
    [InlineData(Int32.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public void TestConstructor_OtpExpiryMinutes_Set_Invalid(int newOtpExpiryMinutes)
    {
        // Arrange
        AuthenticationOptions options = new();
        int otpExpiryMinutes = options.OtpExpiryMinutes;

        // Act
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                options.OtpExpiryMinutes = newOtpExpiryMinutes);

        // Assert
        Assert.Equal(newOtpExpiryMinutes, ex.ActualValue);
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(otpExpiryMinutes, options.OtpExpiryMinutes);
    }

    [InlineData(1)]
    [InlineData(Int32.MaxValue)]
    [Theory]
    public void TestConstructor_OtpExpiryMinutes_Set_Valid(int otpExpiryMinutes)
    {
        // Arrange
        AuthenticationOptions options = new();

        // Act
        options.OtpExpiryMinutes = otpExpiryMinutes;

        // Assert
        Assert.Equal(otpExpiryMinutes, options.OtpExpiryMinutes);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        AuthenticationOptions options = new();

        // Assert
        Assert.False(options.OtpExpiryMinutes <= 0);
    }
}
