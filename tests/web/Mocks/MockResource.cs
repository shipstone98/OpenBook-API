using System;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockResource : IResource
{
    internal Func<Guid> _creatorId;

    Guid IResource.CreatorId => this._creatorId();

    internal MockResource() =>
        this._creatorId = () => throw new NotImplementedException();
}
