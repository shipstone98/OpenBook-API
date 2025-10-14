using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

using Shipstone.OpenBook.Api.Web.Controllers;

namespace Shipstone.OpenBook.Api.Web;

internal sealed class OpenBookControllerFeatureProvider
    : ControllerFeatureProvider
{
    private readonly IReadOnlySet<Type> _types;

    internal OpenBookControllerFeatureProvider() =>
        this._types = new HashSet<Type>
        {
            typeof (AccountController),
            typeof (FollowingController),
            typeof (PasswordController),
            typeof (PostController),
            typeof (UserController)
        };

    protected sealed override bool IsController(TypeInfo typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);
        return this._types.Contains(typeInfo);
    }
}
