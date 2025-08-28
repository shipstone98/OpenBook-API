using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Services;

internal interface INormalizationService
{
    String Normalize(String s);
}
