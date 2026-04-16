using System;

namespace Shipstone.OpenBook.Api.Core.Services;

internal interface IValidationService
{
    String ValidateUserName(String userName);
}
