using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockRepository : IRepository
{
    internal Func<IPostRepository> _postsFunc;
    internal Func<IRoleRepository> _rolesFunc;
    internal Action _saveAction;
    internal Func<IUserRefreshTokenRepository> _userRefreshTokensFunc;
    internal Func<IUserRoleRepository> _userRolesFunc;
    internal Func<IUserRepository> _usersFunc;

    IPostRepository IRepository.Posts => this._postsFunc();
    IRoleRepository IRepository.Roles => this._rolesFunc();

    IUserRefreshTokenRepository IRepository.UserRefreshTokens =>
        this._userRefreshTokensFunc();

    IUserRoleRepository IRepository.UserRoles => this._userRolesFunc();
    IUserRepository IRepository.Users => this._usersFunc();

    public MockRepository()
    {
        this._postsFunc = () => throw new NotImplementedException();
        this._rolesFunc = () => throw new NotImplementedException();
        this._saveAction = () => throw new NotImplementedException();

        this._userRefreshTokensFunc = () =>
            throw new NotImplementedException();

        this._userRolesFunc = () => throw new NotImplementedException();
        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
