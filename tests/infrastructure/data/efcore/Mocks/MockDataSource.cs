using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockDataSource : IDataSource
{
    internal Action _saveAction;
    internal Func<IDataSet<UserEntity>> _usersFunc;

    IDataSet<UserEntity> IDataSource.Users => this._usersFunc();

    public MockDataSource()
    {
        this._saveAction = () => throw new NotImplementedException();
        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
