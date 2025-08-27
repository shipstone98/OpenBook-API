using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockRepository : IRepository
{
    internal Action _saveAction;
    internal Func<IUserRepository> _usersFunc;

    IUserRepository IRepository.Users => this._usersFunc();

    public MockRepository()
    {
        this._saveAction = () => throw new NotImplementedException();
        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
