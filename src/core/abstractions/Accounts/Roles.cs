using System;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines constants for the well-known roles.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Specifies the name of the <c>Administrator</c> role. This field is constant.
    /// </summary>
    public const String Administrator = "Administrator";

    /// <summary>
    /// Specifies the ID of the <c>Administrator</c> role. This field is constant.
    /// </summary>
    public const long AdministratorId = 2;

    /// <summary>
    /// Specifies the name of the <c>System Administrator</c> role. This field is constant.
    /// </summary>
    public const String SystemAdministrator = "System Administrator";

    /// <summary>
    /// Specifies the ID of the <c>System Administrator</c> role. This field is constant.
    /// </summary>
    public const long SystemAdministratorId = 3;

    /// <summary>
    /// Specifies the name of the <c>User</c> role. This field is constant.
    /// </summary>
    public const String User = "User";

    /// <summary>
    /// Specifies the ID of the <c>User</c> role. This field is constant.
    /// </summary>
    public const long UserId = 1;
}
