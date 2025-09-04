using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthorizationTest.Mocks;

internal sealed class MockCreatableEntity<TId> : CreatableEntity<TId>
    where TId : struct
{ }
