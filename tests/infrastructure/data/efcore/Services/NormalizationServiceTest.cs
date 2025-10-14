using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Services;

public sealed class NormalizationServiceTest
{
    private readonly MockHMAC _hmac;
    private readonly INormalizationService _normalization;

    public NormalizationServiceTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureDataEntityFrameworkCore();
        MockHMAC hmac = new();
        services.AddSingleton<HMAC>(hmac);
        IServiceProvider provider = new MockServiceProvider(services);
        this._hmac = hmac;

        this._normalization =
            provider.GetRequiredService<INormalizationService>();
    }

    [Fact]
    public void TestNormalize_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                this._normalization.Normalize(null!));

        // Assert
        Assert.Equal("s", ex.ParamName);
    }

    [Fact]
    public void TestNormalize_Valid()
    {
        // Arrange
        const String S = "Hello, world!";
        String sUpper = S.ToUpper();
        String sLower = S.ToLower();
        this._hmac._hashCoreAction = (_, _, _) => { };
        this._hmac._hashFinalFunc = Array.Empty<byte>;
        this._hmac._initializeAction = () => { };

        // Act
        String sUpperActual = this._normalization.Normalize(sUpper);
        String sLowerActual = this._normalization.Normalize(sLower);

        // Assert
        Assert.NotNull(sUpperActual);
        Assert.NotNull(sLowerActual);

#pragma warning disable xUnit2010
        Assert.True(String.Equals(sUpperActual, sLowerActual));
#pragma warning restore xUnit2010
    }
}
