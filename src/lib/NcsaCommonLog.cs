using System;
using System.Net;

using Shipstone.Utilities.IO;

namespace Shipstone.AspNetCore.Http;

internal sealed class NcsaCommonLog : INcsaCommonLog
{
    private readonly String? _authenticatedUser;
    private readonly long _contentLength;
    private readonly IPAddress? _host;
    private readonly DateTime _received;
    private readonly String _requestLine;
    private readonly HttpStatusCode _statusCode;

    String? INcsaCommonLog.AuthenticatedUser => this._authenticatedUser;
    Nullable<long> INcsaCommonLog.ContentLength => this._contentLength;
    IPAddress? INcsaCommonLog.Host => this._host;
    String? INcsaCommonLog.Identity => null;
    Nullable<DateTime> INcsaCommonLog.Received => this._received;
    String? INcsaCommonLog.RequestLine => this._requestLine;
    Nullable<HttpStatusCode> INcsaCommonLog.StatusCode => this._statusCode;

    internal NcsaCommonLog(
        IPAddress? host,
        String? authenticatedUser,
        DateTime received,
        String requestLine,
        HttpStatusCode statusCode,
        long contentLength
    )
    {
        this._authenticatedUser = authenticatedUser;
        this._contentLength = contentLength;
        this._host = host;
        this._received = received;
        this._requestLine = requestLine;
        this._statusCode = statusCode;
    }
}
