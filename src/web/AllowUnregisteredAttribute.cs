using System;

namespace Shipstone.OpenBook.Api.Web;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class AllowUnregisteredAttribute
    : Attribute, IAllowUnregistered
{ }
