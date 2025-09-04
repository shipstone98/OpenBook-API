using System;
using System.Net;
using System.Text;

namespace Shipstone.Utilities.IO;

public static class NcsaCommonLogExtensions
{
    public static String ToString(
        this INcsaCommonLog log,
        IFormatProvider? provider = null
    )
    {
        ArgumentNullException.ThrowIfNull(log);
        Nullable<DateTime> received = log.Received;
        String? receivedString;

        if (received.HasValue)
        {
            if (provider is null)
            {
                receivedString =
                    received.Value.ToString("dd/MM/yyyy HH:mm:ss zzz");
            }

            else
            {
                receivedString = received.Value.ToString(provider);
            }

            receivedString = $"[{receivedString}]";
        }

        else
        {
            receivedString = null;
        }

        String? requestLine = log.RequestLine;
        Nullable<HttpStatusCode> statusCode = log.StatusCode;

        Nullable<int> statusCodeInt32 =
            statusCode.HasValue ? (int) statusCode.Value : null;

        StringBuilder sb = new();

        return sb
            .AppendNcsaCommonLog(log.Host)
            .AppendNcsaCommonLog(log.Identity)
            .AppendNcsaCommonLog(log.AuthenticatedUser)
            .AppendNcsaCommonLog(receivedString)
            .AppendNcsaCommonLog(requestLine is null ? null : $"\"{requestLine}\"")
            .AppendNcsaCommonLog(statusCodeInt32)
            .AppendNcsaCommonLog(log.ContentLength)
            .ToString();
    }
}
