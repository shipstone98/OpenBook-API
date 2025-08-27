using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockMvcBuilder : IMvcBuilder
{
    internal Func<ApplicationPartManager> _partManagerFunc;

    ApplicationPartManager IMvcBuilder.PartManager => this._partManagerFunc();

    IServiceCollection IMvcBuilder.Services =>
        throw new NotImplementedException();

    internal MockMvcBuilder() =>
        this._partManagerFunc = () => throw new NotImplementedException();
}
