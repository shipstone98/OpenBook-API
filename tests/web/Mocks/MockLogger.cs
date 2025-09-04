using System;
using Microsoft.Extensions.Logging;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockLogger<TCategoryName> : ILogger<TCategoryName>
{
    internal Func<LogLevel, bool> _isEnabledFunc;

    public MockLogger() =>
        this._isEnabledFunc = _ => throw new NotImplementedException();

    IDisposable? ILogger.BeginScope<TState>(TState state) =>
        throw new NotImplementedException();

    bool ILogger.IsEnabled(LogLevel logLevel) => this._isEnabledFunc(logLevel);

    void ILogger.Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, String> formatter
    ) =>
        throw new NotImplementedException();
}
