using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.WebApi;

internal sealed class StubMailService : IMailService
{
    Task IMailService.SendRegistrationAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    Task IMailService.SendUnregistrationAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
