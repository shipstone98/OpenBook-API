using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Shipstone.Utilities.IO;

namespace Shipstone.AspNetCore.Http;

internal sealed class NcsaCommonLoggingMiddleware
    : IAsyncDisposable, IDisposable, IMiddleware
{
    private readonly TextWriter _writer;

    internal NcsaCommonLoggingMiddleware(TextWriter writer) =>
        this._writer = writer;

    private async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        HttpResponse response = context.Response;
        LengthStream body = new(response.Body);
        response.Body = body;
        await next(context);
        HttpRequest request = context.Request;

        INcsaCommonLog log =
            new NcsaCommonLog(
                context.Connection.RemoteIpAddress,
                context.User.Identity?.Name,
                DateTime.Now,
                $"{request.Method} {request.Path} {request.Protocol}",
                (HttpStatusCode) response.StatusCode,
                body._length
            );

        ReadOnlyMemory<char> chars =
            log
                .ToString(null)
                .ToCharArray();

        await this._writer.WriteLineAsync(chars);
        await this._writer.FlushAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync() => this._writer.DisposeAsync();
    void IDisposable.Dispose() => this._writer.Dispose();

    Task IMiddleware.InvokeAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        return this.InvokeAsync(context, next);
    }
}
