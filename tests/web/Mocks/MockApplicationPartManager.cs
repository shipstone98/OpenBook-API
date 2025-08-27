using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockApplicationPartManager : ApplicationPartManager
{
    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override String ToString() =>
        throw new NotImplementedException();
}
