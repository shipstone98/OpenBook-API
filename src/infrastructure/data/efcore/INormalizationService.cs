using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

internal interface INormalizationService
{
    String Normalize(String s);
}
