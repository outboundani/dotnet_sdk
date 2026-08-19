using System.Text;

namespace OutboundIQ;

/// <summary>
/// Builds request URLs from a base address, a path, and optional query parameters.
/// </summary>
internal static class QueryString
{
    /// <summary>
    /// Joins <paramref name="baseUrl"/> and <paramref name="path"/> and appends any query
    /// parameters whose value is not <see langword="null"/>.
    /// </summary>
    public static string BuildUrl(string baseUrl, string path, IReadOnlyList<KeyValuePair<string, string?>>? query)
    {
        var builder = new StringBuilder(baseUrl);
        builder.Append(path);

        if (query is null || query.Count == 0)
        {
            return builder.ToString();
        }

        var first = true;
        foreach (var (key, value) in query)
        {
            // Unset parameters are omitted entirely, matching the TypeScript SDK, where an
            // undefined value is never written to the URLSearchParams.
            if (value is null)
            {
                continue;
            }

            builder.Append(first ? '?' : '&');
            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
            first = false;
        }

        return builder.ToString();
    }

    /// <summary>Removes any trailing slashes from a base address.</summary>
    public static string NormalizeBaseUrl(Uri baseUrl) =>
        baseUrl.AbsoluteUri.TrimEnd('/');
}
