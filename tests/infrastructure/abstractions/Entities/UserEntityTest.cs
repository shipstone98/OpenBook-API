using System;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.AbstractionsTest.Entities;

public sealed class UserEntityTest
{
    public void TestEmailAddress_Initialize_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                new UserEntity
                {
                    EmailAddress = null!
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    public void TestEmailAddress_Initialize_Valid()
    {
        // Arrange
        const String EMAIL_ADDRESS = "john.doe@contoso.com";

        // Act
        UserEntity user = new UserEntity
        {
            EmailAddress = EMAIL_ADDRESS
        };

        // Assert
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
    }

    public void TestConstructor()
    {
        // Act
        UserEntity user = new();

        // Assert
        Assert.NotNull(user.EmailAddress);
        Assert.NotNull(user.EmailAddressNormalized);
        Assert.NotNull(user.Forename);

        Assert.True(
            user.Otp is null
            || user.Otp.Length <= Constants.UserOtpMaxLength
        );

        Assert.NotNull(user.Surname);
        Assert.NotNull(user.UserName);
    }
}
