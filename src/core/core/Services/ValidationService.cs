using System;
using System.Text.RegularExpressions;

namespace Shipstone.OpenBook.Api.Core.Services;

internal sealed partial class ValidationService : IValidationService
{
    private readonly Regex _userNamePattern;

    public ValidationService() =>
        this._userNamePattern = ValidationService.GenerateUserNamePattern();

    String IValidationService.ValidateUserName(String userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        userName = userName.Trim();
        int length = userName.Length;

        if (
            !this._userNamePattern.IsMatch(userName)
            || length > Constants.UserNameMaxLength
            || length < Constants.UserNameMinLength
        )
        {
            throw new ArgumentException(
                $"{nameof (userName)} is not a valid user name.",
                nameof (userName)
            );
        }

        return userName;
    }

    [GeneratedRegex(@"^[a-z][a-z0-9_]+$", RegexOptions.IgnoreCase)]
    private static partial Regex GenerateUserNamePattern();
}
