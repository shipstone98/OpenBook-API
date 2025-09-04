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
        IPAddress? host = context.Connection.RemoteIpAddress;
        DateTime received = DateTime.Now;
        HttpRequest request = context.Request;

        String requestLine =
            $"{request.Method} {request.Path} {request.Protocol}";

        await next(context);
        String? authenticatedUser = context.User.Identity?.Name;
        HttpResponse response = context.Response;
        HttpStatusCode statusCode = (HttpStatusCode) response.StatusCode;
        Nullable<long> contentLength = response.ContentLength;

        if (!contentLength.HasValue)
        {
            try
            {
                contentLength = response.Body.Length;
            }

            catch (NotSupportedException)
            {
                contentLength = 0;
            }
        }

        INcsaCommonLog log =
            new NcsaCommonLog(
                host,
                authenticatedUser,
                received,
                requestLine,
                statusCode,
                contentLength.Value
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
