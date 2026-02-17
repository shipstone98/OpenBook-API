using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Pagination;
using Shipstone.Extensions.Security;
using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IDataSource _dataSource;
    private readonly INormalizationService _normalization;
    private readonly IPaginationService _pagination;

    public UserRepository(
        IDataSource dataSource,
        INormalizationService normalization,
        IPaginationService pagination
    )
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        ArgumentNullException.ThrowIfNull(normalization);
        ArgumentNullException.ThrowIfNull(pagination);
        this._dataSource = dataSource;
        this._normalization = normalization;
        this._pagination = pagination;
    }

    private async Task<UserEntity?> RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        String emailAddressNormalized =
            this._normalization.Normalize(emailAddress);

        IEnumerable<UserEntity> users =
            await this._dataSource.Users
                .Where(u =>
                    emailAddressNormalized.Equals(u.EmailAddressNormalized))
                .ToArrayAsync(cancellationToken);

        return users.FirstOrDefault(u => emailAddress.Equals(u.EmailAddress));
    }

    private async Task<UserEntity?> RetrieveForNameAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        String userNameNormalized = this._normalization.Normalize(userName);

        IEnumerable<UserEntity> users =
            await this._dataSource.Users
                .Where(u => userNameNormalized.Equals(u.UserNameNormalized))
                .ToArrayAsync(cancellationToken);

        return users.FirstOrDefault(u => userName.Equals(u.UserName));
    }

    Task IUserRepository.CreateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        return this._dataSource.Users.SetStateAsync(
            user,
            DataEntityState.Created,
            cancellationToken
        );
    }

#warning Not tested
    Task<IReadOnlyPaginatedList<UserEntity>> IUserRepository.ListAsync(CancellationToken cancellationToken)
    {
        IQueryable<UserEntity> query = this._dataSource.Users.AsNoTracking();

        return this._pagination.GetPageOrFirstAsync(
            query,
            cancellationToken
        );
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        return this.RetrieveAsync(emailAddress, cancellationToken);
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(id, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (id)} is equal to Guid.Empty.",
                nameof (id)
            );
        }

        return this._dataSource.Users.FirstOrDefaultAsync(
            u => Guid.Equals(id, u.Id),
            cancellationToken
        );
    }

    Task<UserEntity?> IUserRepository.RetrieveForNameAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.RetrieveForNameAsync(userName, cancellationToken);
    }

    Task IUserRepository.UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        return this._dataSource.Users.SetStateAsync(
            user,
            DataEntityState.Updated,
            cancellationToken
        );
    }
}
