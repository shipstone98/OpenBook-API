using System;

using Shipstone.Extensions.Security;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockNormalizationService : INormalizationService
{
    internal Func<String, String> _normalizeFunc;

    public MockNormalizationService() =>
        this._normalizeFunc = _ => throw new NotImplementedException();

    String INormalizationService.Normalize(String s) => this._normalizeFunc(s);
}
