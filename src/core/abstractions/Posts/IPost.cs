using System;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Represents a post.
/// </summary>
public interface IPost
{
    /// <summary>
    /// Gets the body of the <see cref="IPost" />.
    /// </summary>
    /// <value>The body of the <see cref="IPost" />.</value>
    String Body { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IPost" /> was created.
    /// </summary>
    /// <value>The date and time the <see cref="IPost" /> was created.</value>
    DateTime Created { get; }

    /// <summary>
    /// Gets the email address of the user that created the <see cref="IPost" />.
    /// </summary>
    /// <value>The email address of the user that created the <see cref="IPost" />.</value>
    String CreatorEmailAddress { get; }

    /// <summary>
    /// Gets the name of the user that created the <see cref="IPost" />.
    /// </summary>
    /// <value>The name of the user that created the <see cref="IPost" />.</value>
    String CreatorName { get; }

    /// <summary>
    /// Gets the ID of the <see cref="IPost" />.
    /// </summary>
    /// <value>The ID of the <see cref="IPost" />.</value>
    long Id { get; }

    /// <summary>
    /// Gets the ID of the parent of the <see cref="IPost" />.
    /// </summary>
    /// <value>The ID of the parent of the <see cref="IPost" />, or <c>null</c> if the <see cref="IPost" /> has no parent.</value>
    Nullable<long> ParentId { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IPost" /> was last updated.
    /// </summary>
    /// <value>The date and time the <see cref="IPost" /> was last updated.</value>
    DateTime Updated { get; }
}
