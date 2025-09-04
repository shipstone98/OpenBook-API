using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Shipstone.Utilities.IO;

internal static partial class StringBuilderExtensions
{
    private static readonly Regex _whiteSpacePattern;

    static StringBuilderExtensions() =>
        StringBuilderExtensions._whiteSpacePattern =
            StringBuilderExtensions.GenerateWhiteSpacePattern();

    internal static StringBuilder AppendNcsaCommonLog(
        this StringBuilder sb,
        Object? obj,
        String placeholder = "-"
    )
    {
        String? s =
            obj?
                .ToString()?
                .Trim();

        if (s is not null)
        {
            s = StringBuilderExtensions._whiteSpacePattern.Replace(s, " ");
        }

        if (sb.Length > 0)
        {
            sb.Append(' ');
        }

        return String.IsNullOrWhiteSpace(s)
            ? sb.Append(placeholder)
            : sb.Append(s);
    }

    [GeneratedRegex(@"[\s]+")]
    private static partial Regex GenerateWhiteSpacePattern();
}
