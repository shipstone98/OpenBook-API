using System;
using Microsoft.Extensions.Options;

namespace Shipstone.Extensions.Pagination;

/// <summary>
/// Specifies options for pagination requirements.
/// </summary>
public class PaginationOptions : IOptions<PaginationOptions>
{
    private int _maxCount;
    private int _pageIndex;

    public int MaxCount
    {
        get => this._maxCount;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            this._maxCount = value;
        }
    }

    public int PageIndex
    {
        get => this._pageIndex;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
            this._pageIndex = value;
        }
    }

    PaginationOptions IOptions<PaginationOptions>.Value => this;

    public PaginationOptions() => this._maxCount = 10;
}
