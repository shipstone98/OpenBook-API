using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Data;

/// <summary>
/// Defines a method to normalize data.
/// </summary>
public interface INormalizationService
{
    /// <summary>
    /// Normalizes the specified string.
    /// </summary>
    /// <param name="s">The string to normalize.</param>
    /// <returns>The normalized representation of <c><paramref name="s" /></c>.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="s" /></c> is <c>null</c>.</exception>
    String Normalize(String s);
}
