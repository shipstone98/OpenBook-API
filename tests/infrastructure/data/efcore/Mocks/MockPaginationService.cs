using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Pagination;

using Shipstone.Utilities.Collections;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockPaginationService : IPaginationService
{
    Task<IReadOnlyPaginatedList<T>> IPaginationService.GetPageOrFirstAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException();
}
