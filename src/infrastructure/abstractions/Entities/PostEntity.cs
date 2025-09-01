using System;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a post.
/// </summary>
public class PostEntity : CreatableEntity<long>
{
    private String _body;

    public String Body
    {
        get => this._body;

        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length > Constants.PostBodyMaxLength)
            {
                throw new ArgumentException(
                    $"The length of {nameof (value)} is greater than Constants.PostBodyMaxLength.",
                    nameof (value)
                );
            }

            this._body = value;
        }
    }

    /// <summary>
    /// Gets the ID of the parent of the post.
    /// </summary>
    /// <value>The ID of the parent of the post, or <c>null</c> if the post has no parent.</value>
    public Nullable<long> ParentId { get; init; }

    public PostEntity() => this._body = String.Empty;
}
