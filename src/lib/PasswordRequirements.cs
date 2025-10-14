using System;

namespace Shipstone.Extensions.Identity;

[Flags]
public enum PasswordRequirements
{
    None = 0,
    RequiredLength = 1,
    RequiredUniqueChars = 2,
    RequireUppercase = 4,
    RequireLowercase = 8,
    RequireDigit = 16,
    RequireNonAlphanumeric = 32
}
