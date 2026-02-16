using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Core;

internal static class TaskExtensions
{
    internal static async IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(
        this Task<TSource[]> source
    )
    {
        foreach (TSource item in await source)
        {
            yield return item;
        }
    }
}
