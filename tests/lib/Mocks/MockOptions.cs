using Microsoft.Extensions.Options;

namespace Shipstone.Test.Mocks;

internal sealed class MockOptions<TOptions> : IOptions<TOptions>
    where TOptions : class
{
    private readonly TOptions _value;

    TOptions IOptions<TOptions>.Value => this._value;

    internal MockOptions(TOptions val) => this._value = val;
}
