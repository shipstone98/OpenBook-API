using System;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Represents a mutable post. This class cannot be inherited.
/// </summary>
public sealed class PostBuilder
{
    private String _body;
    private Nullable<long> _parentId;

    /// <summary>
    /// Gets or sets the body for the new post.
    /// </summary>
    /// <value>The body for the new post.</value>
    /// <exception cref="ArgumentException">The property is set and the length of the value is greater than <see cref="Constants.PostBodyMaxLength" />.</exception>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String Body
    {
        get => this._body;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            value = value.Trim();

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
    /// Gets or sets the parent ID for the new post.
    /// </summary>
    /// <value>The parent ID for the new post.</value>
    /// <exception cref="ArgumentOutOfRangeException">The property is set and the value is less than or equal to 0 (zero).</exception>
    public Nullable<long> ParentId
    {
        get => this._parentId;

        set
        {
            if (value.HasValue && value.Value < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof (value),
                    value,
                    $"{nameof (value)} is less than or equal to 0 (zero)."
                );
            }

            this._parentId = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostBuilder" /> class.
    /// </summary>
    public PostBuilder() => this._body = "Hello, world!";
}
