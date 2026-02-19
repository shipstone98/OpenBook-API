using System;
using System.Collections.Generic;

namespace Shipstone.OpenBook.Api.Core;

internal sealed class Resource : IResource
{
    private readonly Guid _creatorId;
    private readonly IReadOnlySet<String> _creatorRoles;

    Guid IResource.CreatorId => this._creatorId;
    IReadOnlySet<String> IResource.CreatorRoles => this._creatorRoles;

    internal Resource(Guid creatorId, IReadOnlySet<String> creatorRoles)
    {
        this._creatorId = creatorId;
        this._creatorRoles = creatorRoles;
    }
}
