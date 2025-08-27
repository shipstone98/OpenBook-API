namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

/// <summary>
/// Specifies the states of a data entity.
/// </summary>
public enum DataEntityState
{
    /// <summary>
    /// Represents the <c>Created</c> state.
    /// </summary>
    Created,

    /// <summary>
    /// Represents the <c>Updated</c> state.
    /// </summary>
    Updated,

    /// <summary>
    /// Represents the <c>Deleted</c> state.
    /// </summary>
    Deleted
}
