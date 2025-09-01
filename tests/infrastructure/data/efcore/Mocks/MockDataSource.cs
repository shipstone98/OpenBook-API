using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockDataSource : IDataSource
{
    internal Func<IDataSet<PostEntity>> _postsFunc;
    internal Func<IDataSet<RoleEntity>> _rolesFunc;
    internal Action _saveAction;
    internal Func<IDataSet<UserRefreshTokenEntity>> _userRefreshTokensFunc;
    internal Func<IDataSet<UserEntity>> _usersFunc;

    IDataSet<PostEntity> IDataSource.Posts => this._postsFunc();
    IDataSet<RoleEntity> IDataSource.Roles => this._rolesFunc();

    IDataSet<UserRefreshTokenEntity> IDataSource.UserRefreshTokens =>
        this._userRefreshTokensFunc();

    IDataSet<UserRoleEntity> IDataSource.UserRoles =>
        throw new NotImplementedException();

    IDataSet<UserEntity> IDataSource.Users => this._usersFunc();

    public MockDataSource()
    {
        this._rolesFunc = () => throw new NotImplementedException();
        this._saveAction = () => throw new NotImplementedException();

        this._userRefreshTokensFunc = () =>
            throw new NotImplementedException();

        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
