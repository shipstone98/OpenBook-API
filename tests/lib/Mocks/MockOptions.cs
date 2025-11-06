using System;
using Microsoft.Extensions.Options;

namespace Shipstone.Test.Mocks;

public class MockOptions<TOptions> : IOptions<TOptions>
    where TOptions : class
{
    public Func<TOptions> _valueFunc;

    TOptions IOptions<TOptions>.Value => this._valueFunc();

    public MockOptions() =>
        this._valueFunc = () => throw new NotImplementedException();
}
