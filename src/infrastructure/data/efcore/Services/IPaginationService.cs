using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Services;

internal interface IPaginationService
{
    Task<IReadOnlyPaginatedList<T>> GetPageOrFirstAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken
    );
}
