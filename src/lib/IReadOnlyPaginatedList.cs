using System.Collections.Generic;

namespace Shipstone.Utilities.Collections;

public interface IReadOnlyPaginatedList<out T> : IReadOnlyList<T>
{
    int PageCount { get; }
    int PageIndex { get; }
    int TotalCount { get; }
}
