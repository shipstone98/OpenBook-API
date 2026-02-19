using System;
using System.Collections.Generic;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Test.Mocks;

public sealed class MockResource : IResource
{
    public Func<Guid> _creatorIdFunc;
    public Func<IReadOnlySet<String>> _creatorRolesFunc;

    Guid IResource.CreatorId => this._creatorIdFunc();
    IReadOnlySet<String> IResource.CreatorRoles => this._creatorRolesFunc();

    public MockResource()
    {
        this._creatorIdFunc = () => throw new NotImplementedException();
        this._creatorRolesFunc = () => throw new NotImplementedException();
    }
}
