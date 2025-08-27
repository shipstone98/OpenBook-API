using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shipstone.AspNetCore.Http;

public abstract class ExceptionHandlingMiddleware<TException>
    : IMiddleware
    where TException : notnull, Exception
{
    private readonly int _statusCode;

    protected ExceptionHandlingMiddleware(int statusCode) =>
        this._statusCode = statusCode;

    private async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }

        catch (TException)
        {
            HttpResponse response = context.Response;

            if (!response.HasStarted)
            {
                response.StatusCode = this._statusCode;
            }
        }
    }

    Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        return this.InvokeAsync(context, next);
    }
}
