using System;
using Microsoft.Extensions.Options;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockOptionsSnapshot<TOptions>
    : MockOptions<TOptions>, IOptionsSnapshot<TOptions>
    where TOptions : class
{
    TOptions IOptionsSnapshot<TOptions>.Get(String? name) =>
        throw new NotImplementedException();
}
