using System;
using System.Net;

namespace Shipstone.Utilities.IO;

public interface INcsaCommonLog
{
    String? AuthenticatedUser { get; }
    Nullable<long> ContentLength { get; }
    IPAddress? Host { get; }
    String? Identity { get; }
    Nullable<DateTime> Received { get; }
    String? RequestLine { get; }
    Nullable<HttpStatusCode> StatusCode { get; }
}
