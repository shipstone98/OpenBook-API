using System;
using Xunit;

using Shipstone.OpenBook.Api.Core.Posts;

namespace Shipstone.OpenBook.Api.Core.AbstractionsTest.Posts;

public sealed class PostBuilderTest
{
#region Body property
    [Fact]
    public void TestBody_Set_Invalid_NotNull()
    {
        // Arrange
        PostBuilder builder = new();
        String body = builder.Body;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                builder.Body =
                    String.Empty.PadRight(
                        Constants.PostBodyMaxLength + 1,
                        '0'
                    ));

        // Assert
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(body, builder.Body);
    }

    [Fact]
    public void TestBody_Set_Invalid_Null()
    {
        // Arrange
        PostBuilder builder = new();
        String body = builder.Body;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() => builder.Body = null!);

        // Assert
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(body, builder.Body);
    }

    [Fact]
    public void TestBody_Set_Valid()
    {
        // Arrange
        PostBuilder builder = new();
        String body = String.Empty.PadRight(Constants.PostBodyMaxLength, '0');

        // Act
        builder.Body = body;

        // Assert
        Assert.Equal(body, builder.Body);
    }
#endregion

    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public void TestParentId_Set_Invalid(long newParentId)
    {
        // Arrange
        PostBuilder builder = new();
        Nullable<long> parentId = builder.ParentId;

        // Act
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.ParentId = newParentId);

        // Assert
        Assert.Equal(newParentId, ex.ActualValue);
        Assert.Equal("value", ex.ParamName);
        Assert.Equal(parentId, builder.ParentId);
    }

    [InlineData(null)]
    [InlineData(1L)]
    [InlineData(Int64.MaxValue)]
    [Theory]
    public void TestParentId_Set_Valid(Nullable<long> parentId)
    {
        // Arrange
        PostBuilder builder = new();

        // Act
        builder.ParentId = parentId;

        // Assert
        Assert.Equal(parentId, builder.ParentId);
    }

    [Fact]
    public void TestConstructor()
    {
        // Act
        PostBuilder builder = new();

        // Assert
        Assert.False(builder.Body.Length > Constants.PostBodyMaxLength);

        if (builder.ParentId.HasValue)
        {
            Assert.False(builder.ParentId.Value < 1);
        }
    }
}
