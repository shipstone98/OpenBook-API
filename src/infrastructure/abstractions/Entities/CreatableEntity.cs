using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents an entity with a simple primary key and creator ID.
/// </summary>
/// <typeparam name="TId">The type of the ID of the entity.</typeparam>
public abstract class CreatableEntity<TId> : Entity<TId> where TId : struct
{
    /// <summary>
    /// Gets or initializes the ID of user that created the entity.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the entity.</value>
    public Guid CreatorId { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatableEntity{TId}" /> class.
    /// </summary>
    protected CreatableEntity() { }
}
