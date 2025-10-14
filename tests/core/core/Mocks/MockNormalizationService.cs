using System;

using Shipstone.OpenBook.Api.Infrastructure.Data;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockNormalizationService : INormalizationService
{
    internal Func<String, String> _normalizeFunc;

    public MockNormalizationService() =>
        this._normalizeFunc = _ => throw new NotImplementedException();

    String INormalizationService.Normalize(String s) => this._normalizeFunc(s);
}
