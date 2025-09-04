using System;
using Microsoft.Extensions.Options;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockOptionsSnapshot<TOptions>
    : IOptionsSnapshot<TOptions>
    where TOptions : class
{
    internal Func<TOptions> _valueFunc;

    TOptions IOptions<TOptions>.Value => this._valueFunc();

    public MockOptionsSnapshot() =>
        this._valueFunc = () => throw new NotImplementedException();

    TOptions IOptionsSnapshot<TOptions>.Get(String? name) =>
        throw new NotImplementedException();
}
